using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TENamespace.health;
using TerraEngineer;
using TerraEngineer.entities.mobs;

// TODO:
// Make it display preview in editor

public partial class Fluid : Node2D
{
	[Export] private Vector2I size = new Vector2I(100, 32);
	[Export] private int springsAmountPer10Px = 1;
	[Export] private float forceOnContact = 80;
	
	[Export] private PackedScene fluidSpringScene;
	[Export] private Polygon2D displayPolygon;
	[Export] private Line2D surfaceLine;
	[Export] private CollisionShape2D collisionShape;
	
	private List<FluidSpring> fluidSprings = new List<FluidSpring>();
	private bool initialized = false;
	
	public override void _Ready()
	{
		if (!initialized)
		{
			setupSprings();
			setupCollisions();
			initialized = true;	
		}
	}

	private void setupSprings()
	{
		createFluidSpring(new Vector2(0, 0)); // Left coast
		
		int springsAmount = (int)Math.Ceiling( (size.X / 10.0) * springsAmountPer10Px );
		
		// size.X = How much space we need if we would want to distribute all springs
		// (size.X - (size.X/springsAmount-2) ) = How much space we need if we want to distribute all springs -2
		int spaceBetween = (size.X - (size.X/springsAmount-2) ) / (springsAmount-2);
		
		int xOffset = spaceBetween;
		for (int i = 0; i < springsAmount-2; i++)
		{
			createFluidSpring(new Vector2(xOffset, 0));
			
			xOffset += spaceBetween;
		}
		
		createFluidSpring(new Vector2(size.X, 0)); // Right coast
		
		for (int i = 0; i < springsAmount; i++)
		{
			fluidSprings[i].SetupNeighbors( 
				i-1 >= 0 ? fluidSprings[i-1] : null,
				i+1 < fluidSprings.Count ? fluidSprings[i+1] : null
				);
		}
	}

	private void setupCollisions()
	{
		RectangleShape2D shape = new RectangleShape2D();
		shape.Size = size;
		collisionShape.Position += size / 2;
		collisionShape.Shape = shape;
	}

	private void createFluidSpring(Vector2 position)
	{
		FluidSpring springInstance = (FluidSpring)fluidSpringScene.Instantiate();
		springInstance.Position = position;
		AddChild(springInstance);
		fluidSprings.Add(springInstance);
	}

	private void reset()
	{
		foreach (FluidSpring spring in fluidSprings)
		{
			spring.QueueFree();
		}
		fluidSprings.Clear();
	}
	
	public override void _Process(double delta)
	{
		// Order of points counts!
		List<Vector2> bodyPoints = new List<Vector2>();
		List<Vector2> surfacePoints = new List<Vector2>();
		bodyPoints.Add(new  Vector2(0, size.Y));
		
		foreach (FluidSpring spring in fluidSprings)
		{
			bodyPoints.Add(spring.Position);
			surfacePoints.Add(spring.Position);
		}
		
		bodyPoints.Add(new  Vector2(size.X, size.Y));
		
		displayPolygon.SetPolygon(bodyPoints.ToArray());
		surfaceLine.SetPoints(surfacePoints.ToArray());
	}
	
	private void _onBodyEntered(Node2D body)
	{
		addForce((Entity)body, true);
	}

	private void _onBodyExited(Node2D body)
	{
		addForce((Entity)body, false);
	}
	
	private void addForce(Entity source, bool entering)
	{
		float minDist = float.MaxValue;
		FluidSpring closestSpring = null;
		foreach (FluidSpring spring in fluidSprings)
		{
			float dist = source.GlobalPosition.DistanceSquaredTo(spring.GlobalPosition);
			
			if(dist < minDist)
			{
				minDist = dist;
				closestSpring = spring;
			}
		}
		
		if(closestSpring == null)
			throw new Exception("Body fell into fluid, but a fluid spring was not found");
		
		closestSpring.AddExternalForce(entering ? forceOnContact : -forceOnContact);
	}
	
}
