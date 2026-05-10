using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using TENamespace.basic.damage_overtime;
using TENamespace.health;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.objects;

[Tool]
public partial class Fluid : StaticBody2D
{
	[Export] private Vector2 Size
	{
		get => _size;
		set
		{
			_size.X = ((int)MathF.Round(value.X / 10f)) * 10;
			_size.Y = ((int)MathF.Round(value.Y));
			resetSprings();
			updateDisplayPolygon();
		}
	}
	private Vector2I _size = new Vector2I(100, 100);
	
	[Export] private Biomes CurrentBiome 
	{
		get => _currentBiome;
		set
		{
			changeColors(value);
			_currentBiome = value;
		}
	}
	private Biomes _currentBiome = Biomes.Forest;
	
	[Export] private int springsAmountPer10Px = 3;
	
	[Export] private PackedScene fluidSpringScene;
	[Export] private Polygon2D displayPolygon;
	[Export] private Line2D surfaceLine;
	[Export] private CollisionShape2D collisionShape;
	
	private List<FluidSpring> fluidSprings = new List<FluidSpring>();
	
	// (Color, Color) = (Surface, Body)
	private readonly Dictionary<Biomes, (Color, Color)> biomeColors = new ()
	{
		{ Biomes.Forest, (new Color("#4badff"), new Color("#3972ff73")) },
		{ Biomes.Ice, (new Color("#d9edff"), new Color("#8fd3ffbd")) },
		{ Biomes.Mushroom, (new Color("#91db69"), new Color("#aed957a6")) },
	};
	
	private List<Vector2> frozenBodyPoints = new List<Vector2>();
	private List<Vector2> frozenSurfacePoints = new List<Vector2>();
	private List<float> originalDamping = new List<float>();
	private CollisionShape2D solidCollisionShape;
	private const float AcidDamping = 0.5f;
	
	public override void _Ready()
	{
		resetSprings();
		
		#if TOOLS
		if(Engine.IsEditorHint())
			return;
		#endif
		
		setupCollisions();
		Terraform(_currentBiome);
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
		#if TOOLS
		if(Engine.IsEditorHint())
			return;
		#endif
		
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
	
	private void updateDisplayPolygon()
	{
		// Safety check for uninitialized export variables
		if (displayPolygon == null || surfaceLine == null)
			return;
		
		// Don't update visuals if frozen
		if (_currentBiome == Biomes.Ice)
		{
			displayPolygon.SetPolygon(frozenBodyPoints.ToArray());
			surfaceLine.SetPoints(frozenSurfacePoints.ToArray());
			return;
		}
		
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
	
	public override void _Process(double delta)
	{
		#if TOOLS
		if(Engine.IsEditorHint())
			return;
		#endif
		
		updateDisplayPolygon();
	}
	
	private void _onBodyEntered(Node2D body)
	{
		if (_currentBiome == Biomes.Mushroom && body is Entity e)
		{
			onAcidEntered(e);
		}
		addForce(body, true);
	}

	private void _onBodyExited(Node2D body)
	{

		
		if (body is Entity e)
		{
			if (!e.Dead)
			{
				addForce(body, false);		
				
				if (_currentBiome == Biomes.Mushroom)
				{
					onAcidExited(e);
				}
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
			top3Springs[0].AddExternalForce(entering ? e.Weight : -e.Weight, top3Springs[0].BaseSpread);
			top3Springs[1].AddExternalForce(entering ? e.Weight/2 : -e.Weight/2, top3Springs[0].BaseSpread);
			top3Springs[1].AddExternalForce(entering ? e.Weight / 2 : -e.Weight / 2, top3Springs[0].BaseSpread);
			
			e.FellIntoFluid(this);
		}
	}

	#region Terraforming
	public void Terraform(Biomes biome)
	{
		// Exit current biome first
		switch (_currentBiome)
		{
			case Biomes.Forest:
				// Default
				break;
			case Biomes.Ice:
				ExitIce();
				break;
			case Biomes.Mushroom:
				ExitMushroom();
				break;
		}
		
		// Update to new biome
		_currentBiome = biome;
		changeColors(biome);
		
		// Enter new biome
		switch (biome)
		{
			case Biomes.Forest:
				// Default
				break;
			case Biomes.Ice:
				EnterIce();
				break;
			case Biomes.Mushroom:
				EnterMushroom();
				break;
		}
	}
	
	private void EnterIce()
	{
		frozenBodyPoints.Clear();
		frozenSurfacePoints.Clear();
		
		frozenBodyPoints.Add(new Vector2(0, _size.Y));
		foreach (FluidSpring spring in fluidSprings)
		{
			frozenBodyPoints.Add(spring.Position);
			frozenSurfacePoints.Add(spring.Position);
		}
		frozenBodyPoints.Add(new Vector2(_size.X, _size.Y));
		
		// Create solid collision shape (copy of the existing collision)
		solidCollisionShape = new CollisionShape2D();
		RectangleShape2D shape = new RectangleShape2D();
		shape.Size = _size;
		solidCollisionShape.Shape = shape;
		solidCollisionShape.Position = collisionShape.Position;
		AddChild(solidCollisionShape);
	}

	private void ExitIce()
	{
		// Remove solid collision
		if (solidCollisionShape != null && solidCollisionShape.GetParent() == this)
		{
			solidCollisionShape.QueueFree();
			solidCollisionShape = null;
		}
	}

	private void EnterMushroom()
	{
		foreach (FluidSpring spring in fluidSprings)
		{
			spring.Spread = spring.AcidSpread;
		}
	}

	private void ExitMushroom()
	{
		foreach (FluidSpring spring in fluidSprings)
		{
			spring.Spread = spring.BaseSpread;
		}
	}

	private void onAcidEntered(Entity body)
	{
		if (body.CM.HasComponent<Health>() && !body.CM.HasComponent<DamageOvertime>())
		{
			body.CM.AddComponent(new DamageOvertime());
		}
	}

	private void onAcidExited(Entity body)
	{
		if (body.CM.HasComponent<DamageOvertime>())
		{
			body.CM.RemoveComponent<DamageOvertime>();
		}
	}

	private void changeColors(Biomes biome)
	{
		if(surfaceLine == null || displayPolygon == null)
			return;
		
		surfaceLine.DefaultColor = biomeColors[biome].Item1;
		displayPolygon.Color = biomeColors[biome].Item2;
	}
	
	#endregion

}
