using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

// TODO:
// Make it display preview in editor
// Perhaps move the first and last springs to the middle or else it could look weird

public partial class Fluid : Node2D
{
	[Export] private Vector2I size = new Vector2I(100, 32);
	[Export] private int springsAmountPer10Px = 1;
	[Export] private PackedScene fluidSpringScene;
	[Export] private Polygon2D displayPolygon;
	
	private List<FluidSpring> fluidSprings = new List<FluidSpring>();
	private bool initialized = false;
	
	public override void _Ready()
	{
		if (!initialized)
		{
			setup();
			initialized = true;	
		}
		
		fluidSprings[2].AddExternalForce(100);
	}

	private void setup()
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
		List<Vector2> points = new List<Vector2>();
		points.Add(new  Vector2(0, size.Y));
		
		foreach (FluidSpring spring in fluidSprings)
		{
			points.Add(spring.Position);
		}
		
		points.Add(new  Vector2(size.X, size.Y));
		
		displayPolygon.SetPolygon(points.ToArray());
	}
}
