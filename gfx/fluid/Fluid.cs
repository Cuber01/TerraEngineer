using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using TerraEngineer.entities.mobs;

[Tool]
public partial class Fluid : Node2D
{
	[Export] private Vector2 Size
	{
		get => _size;
		set
		{
			_size.X = ((int)MathF.Round(value.X / 10f)) * 10;
			_size.Y = ((int)MathF.Round(value.Y));
			CallDeferred(nameof(resetSprings));
		}
	}
	private Vector2I _size = new Vector2I(100, 100);
	
	[Export] private int springsAmountPer10Px = 3;
	
	[Export] private PackedScene fluidSpringScene;
	[Export] private Polygon2D displayPolygon;
	[Export] private Line2D surfaceLine;
	[Export] private CollisionShape2D collisionShape;
	
	private List<FluidSpring> fluidSprings = new List<FluidSpring>();
	
	public override void _Ready()
	{
		resetSprings();
		
		#if TOOLS
		if(Engine.IsEditorHint())
			return;
		#endif
		
		setupCollisions();
	}
	
	private void resetSprings()
	{
		foreach (FluidSpring spring in fluidSprings)
		{
			spring.QueueFree();
		}
		fluidSprings.Clear();
		
		setupSprings();
	}

	private void setupSprings()
	{
		createFluidSpring(new Vector2(0, 0)); // Left coast
		
		int springsAmount = (int)Math.Ceiling( (_size.X / 10.0) * springsAmountPer10Px );
		
		// How much space is needed to distribute the 2 springs we spawn outside of loop
		// Loss of fraction unavoidable here
		// ReSharper disable once PossibleLossOfFraction
		float spaceTaken = (_size.X / springsAmount - 2) * 2;
		
		float spaceBetween = (_size.X - spaceTaken) / (springsAmount-2);
		
		float xOffset = spaceBetween;
		for (int i = 0; i < springsAmount-2; i++)
		{
			createFluidSpring(new Vector2(xOffset, 0));
			
			xOffset += spaceBetween;
		}
		
		createFluidSpring(new Vector2(_size.X, 0)); // Right coast
		
		#if TOOLS
		if(Engine.IsEditorHint())
			return;
		#endif
		
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
		shape.Size = _size;
		collisionShape.Position += _size / 2;
		collisionShape.Shape = shape;
	}

	private void createFluidSpring(Vector2 position)
	{
		FluidSpring springInstance = (FluidSpring)fluidSpringScene.Instantiate();
		springInstance.Position = position;
		AddChild(springInstance);
		fluidSprings.Add(springInstance);
	}
	
	public override void _Process(double delta)
	{
		// Order of points counts!
		List<Vector2> bodyPoints = new List<Vector2>();
		List<Vector2> surfacePoints = new List<Vector2>();
		bodyPoints.Add(new  Vector2(0, _size.Y));
		
		foreach (FluidSpring spring in fluidSprings)
		{
			bodyPoints.Add(spring.Position);
			surfacePoints.Add(spring.Position);
		}
		
		bodyPoints.Add(new  Vector2(_size.X, _size.Y));
		
		displayPolygon.SetPolygon(bodyPoints.ToArray());
		surfaceLine.SetPoints(surfacePoints.ToArray());
	}
	
	private void _onBodyEntered(Node2D body)
	{
		addForce(body, true);
	}

	private void _onBodyExited(Node2D body)
	{
		if (body is Entity e)
		{
			if (!e.Dead)
			{
				addForce(body, false);		
			}
		}
		else
		{
			addForce(body, false);
		}
	}
	
	private void addForce(Node2D source, bool entering)
	{
		List<FluidSpring> top3Springs = fluidSprings
			.OrderBy(spring => source.GlobalPosition.DistanceSquaredTo(spring.GlobalPosition))
			.Take(3)
			.ToList();
		
		if (source is Entity e)
		{
			top3Springs[0].AddExternalForce(entering ? e.Weight : -e.Weight);
			top3Springs[1].AddExternalForce(entering ? e.Weight/2 : -e.Weight/2);
			top3Springs[1].AddExternalForce(entering ? e.Weight / 2 : -e.Weight / 2);
			
			e.FellIntoFluid(this);
		}
		

	}
	
}
