
using Godot;
using System;
using System.Collections.Generic;
using TENamespace.basic.damage_overtime;
using TENamespace.health;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.objects;

[Tool]
public partial class FluidWithoutSurface : StaticBody2D
{
	[Export] private Vector2 Size
	{
		get => _Size;
		set
		{
			if (this is FluidWithoutSurface)
			{
				_Size.X = (int)value.X;
				_Size.Y = (int)value.Y;
			}
			else
			{
				_Size.X = ((int)MathF.Round(value.X / 10f)) * 10;
				_Size.Y = ((int)MathF.Round(value.Y));
			}

			setupCollisions();
			updateDisplayPolygon();
		}
	}
	protected Vector2I _Size = new Vector2I(100, 100);

	[Export] protected Biomes CurrentBiome 
	{
		get => _CurrentBiome;
		set
		{
			changeColors(value);
			_CurrentBiome = value;
		}
	}
	protected Biomes _CurrentBiome = Biomes.Forest;

	[Export] protected Polygon2D DisplayPolygon;
	[Export] protected CollisionShape2D CollisionShape;

	// last surface points computed in updateDisplayPolygon (used by derived class)
	protected List<Vector2> LastSurfacePoints = new List<Vector2>();

	// (Color, Color) = (Surface, Body)
	protected readonly Dictionary<Biomes, (Color, Color)> BiomeColors = new ()
	{
		{ Biomes.Forest, (new Color("#4badff"), new Color("#3972ff73")) },
		{ Biomes.Ice, (new Color("#d9edff"), new Color("#8fd3ffbd")) },
		{ Biomes.Mushroom, (new Color("#91db69"), new Color("#aed957a6")) },
	};

	protected List<Vector2> FrozenBodyPoints = new List<Vector2>();
	protected List<Vector2> FrozenSurfacePoints = new List<Vector2>();
	protected CollisionShape2D SolidCollisionShape;

	public override void _Ready()
	{
		#if TOOLS
		if(Engine.IsEditorHint())
			return;
		#endif

		setupCollisions();
		Terraform(_CurrentBiome);
	}
	

	private void setupCollisions()
	{
		RectangleShape2D shape = new RectangleShape2D();
		shape.Size = _Size;
		CollisionShape.Position = _Size / 2;
		CollisionShape.Shape = shape;
	}

	protected virtual void updateDisplayPolygon()
	{
		if (DisplayPolygon == null)
			return;
		
		if (_CurrentBiome == Biomes.Ice)
		{
			DisplayPolygon.SetPolygon(FrozenBodyPoints.ToArray());
			LastSurfacePoints = new List<Vector2>(FrozenSurfacePoints);
			return;
		}
		
		Vector2[] rect = new Vector2[] {
			new Vector2(0, _Size.Y),
			new Vector2(0, 0),
			new Vector2(_Size.X, 0),
			new Vector2(_Size.X, _Size.Y)
		};
		DisplayPolygon.SetPolygon(rect);
		LastSurfacePoints = new List<Vector2>();
	}
    
	public override void _Process(double delta)
	{
		updateDisplayPolygon();
	}
    
	protected virtual void _onBodyEntered(Node2D body)
	{
		if (_CurrentBiome == Biomes.Mushroom && body is Entity e)
		{
			onAcidEntered(e);
		}
		// No spring-based forces in surface-less fluids
	}

	protected virtual void _onBodyExited(Node2D body)
	{
		if (body is Entity e)
		{
			if (!e.Dead)
			{
				if (_CurrentBiome == Biomes.Mushroom)
				{
					onAcidExited(e);
				}
			}
		}
	}

	#region Terraforming
	public void Terraform(Biomes biome)
	{
		// Exit current biome first
		switch (_CurrentBiome)
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
		_CurrentBiome = biome;
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
    
	protected virtual void EnterIce()
	{
		// For surface-less fluids we freeze the whole rectangular body
		FrozenBodyPoints.Clear();
		FrozenSurfacePoints.Clear();

		FrozenBodyPoints.Add(new Vector2(0, _Size.Y));
		FrozenBodyPoints.Add(new Vector2(0, 0));
		FrozenBodyPoints.Add(new Vector2(_Size.X, 0));
		FrozenBodyPoints.Add(new Vector2(_Size.X, _Size.Y));

		// Create solid collision shape (copy of the existing collision)
		SolidCollisionShape = new CollisionShape2D();
		RectangleShape2D shape = new RectangleShape2D();
		shape.Size = _Size;
		SolidCollisionShape.Shape = shape;
		SolidCollisionShape.Position = CollisionShape.Position;
		AddChild(SolidCollisionShape);
	}

	protected virtual void ExitIce()
	{
		// Remove solid collision
		if (SolidCollisionShape != null && SolidCollisionShape.GetParent() == this)
		{
			SolidCollisionShape.QueueFree();
			SolidCollisionShape = null;
		}
	}

	protected virtual void EnterMushroom() { }

	protected virtual void ExitMushroom() { }

	protected void onAcidEntered(Entity body)
	{
		if (body.CM.HasComponent<Health>() && !body.CM.HasComponent<DamageOvertime>())
		{
			body.CM.AddComponent(new DamageOvertime());
		}
	}

	protected void onAcidExited(Entity body)
	{
		if (body.CM.HasComponent<DamageOvertime>())
		{
			body.CM.RemoveComponent<DamageOvertime>();
		}
	}

	protected virtual void changeColors(Biomes biome)
	{
		if(DisplayPolygon == null)
			return;
        
		DisplayPolygon.Color = BiomeColors[biome].Item2;
	}
    
	#endregion

}








