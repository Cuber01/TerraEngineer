using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.objects;

[Tool]
public partial class Fluid : FluidWithoutSurface
{
	[Export] protected int springsAmountPer10Px = 3;
	[Export] protected PackedScene fluidSpringScene;
	[Export] protected Line2D surfaceLine;

	protected List<FluidSpring> fluidSprings = new List<FluidSpring>();
	protected List<Vector2> previewSpringPositions = new List<Vector2>();

	public override void _Ready()
	{
		resetSprings();
		base._Ready();
	}

	private void resetSprings()
	{
		if (Engine.IsEditorHint())
		{
			previewSpringPositions.Clear();
			setupSpringsEditor();
			return;
		}

		foreach (FluidSpring spring in fluidSprings)
		{
			spring.CallDeferred(Node.MethodName.QueueFree);
		}
		fluidSprings.Clear();

		setupSprings();
	}

	private void setupSpringsEditor()
	{
		previewSpringPositions.Clear();
		previewSpringPositions.Add(new Vector2(0, 0));

		int springsAmount = Math.Max(3, (int)Math.Ceiling((_Size.X / 10.0) * springsAmountPer10Px));
		double spaceTaken = (((double)_Size.X / springsAmount) - 2.0) * 2.0;
		double spaceBetween = (_Size.X - spaceTaken) / (springsAmount - 2);

		double xOffset = spaceBetween;
		for (int i = 0; i < springsAmount - 2; i++)
		{
			previewSpringPositions.Add(new Vector2((float)xOffset, 0));
			xOffset += spaceBetween;
		}

		previewSpringPositions.Add(new Vector2(_Size.X, 0));
	}

	private void setupSprings()
	{
		createFluidSpring(new Vector2(0, 0)); // Left coast

		int springsAmount = Math.Max(3, (int)Math.Ceiling((_Size.X / 10.0) * springsAmountPer10Px));

		// How much space is needed to distribute the 2 springs we spawn outside of loop
		// Loss of fraction unavoidable here
		// ReSharper disable once PossibleLossOfFraction
		double spaceTaken = (((double)_Size.X / springsAmount) - 2.0) * 2.0;

		double spaceBetween = (_Size.X - spaceTaken) / (springsAmount - 2);

		double xOffset = spaceBetween;
		for (int i = 0; i < springsAmount-2; i++)
		{
			createFluidSpring(new Vector2((float)xOffset, 0));

			xOffset += spaceBetween;
		}

		createFluidSpring(new Vector2(_Size.X, 0)); // Right coast

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

	private void createFluidSpring(Vector2 position)
	{
		FluidSpring springInstance = (FluidSpring)fluidSpringScene.Instantiate();
		springInstance.Position = position;
		CallDeferred(Node.MethodName.AddChild, springInstance);
		fluidSprings.Add(springInstance);
	}

	protected override void updateDisplayPolygon()
	{
		if (DisplayPolygon == null || surfaceLine == null)
			return;

		if (CurrentBiome == Biomes.Ice)
		{
			DisplayPolygon.SetPolygon(FrozenBodyPoints.ToArray());
			surfaceLine.SetPoints(FrozenSurfacePoints.ToArray());
			return;
		}

		// Compute body and surface points from springs
		List<Vector2> bodyPoints = new List<Vector2>();
		List<Vector2> surfacePoints = new List<Vector2>();
		
		bodyPoints.Add(new Vector2(0, _Size.Y));
		
		IEnumerable<Vector2> points = Engine.IsEditorHint()
			? previewSpringPositions
			: fluidSprings.Select(spring => spring.Position);
		
		foreach (Vector2 point in points)
		{
			bodyPoints.Add(point);
			surfacePoints.Add(point);
		}
		
		bodyPoints.Add(new Vector2(_Size.X, _Size.Y));
		
		DisplayPolygon.SetPolygon(bodyPoints.ToArray());
		LastSurfacePoints = surfacePoints;
		
		// Set surface line points
		if (LastSurfacePoints != null && LastSurfacePoints.Count > 0)
			surfaceLine.SetPoints(LastSurfacePoints.ToArray());
		else
			surfaceLine.SetPoints(new Vector2[0]);
	}

	protected override void changeColors(Biomes biome)
	{
		base.changeColors(biome);

		if (surfaceLine == null)
			return;

		surfaceLine.DefaultColor = BiomeColors[biome].Item1;
	}

	protected override void _onBodyEntered(Node2D body)
	{
		if (CurrentBiome == Biomes.Mushroom && body is Entity e)
		{
			onAcidEntered(e);
		}
		addForce(body, true);
	}

	protected override void _onBodyExited(Node2D body)
	{
		if (body is Entity e)
		{
			if (!e.Dead)
			{
				addForce(body, false);

				if (CurrentBiome == Biomes.Mushroom)
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
			top3Springs[2].AddExternalForce(entering ? e.Weight / 2 : -e.Weight / 2, top3Springs[0].BaseSpread);

			e.FellIntoFluid(this);
		}
	}

	protected override void EnterIce()
	{
		FrozenBodyPoints.Clear();
		FrozenSurfacePoints.Clear();

		FrozenBodyPoints.Add(new Vector2(0, _Size.Y));
		foreach (FluidSpring spring in fluidSprings)
		{
			FrozenBodyPoints.Add(spring.Position);
			FrozenSurfacePoints.Add(spring.Position);
		}
		FrozenBodyPoints.Add(new Vector2(_Size.X, _Size.Y));

		// Create solid collision shape (copy of the existing collision)
		SolidCollisionShape = new CollisionShape2D();
		RectangleShape2D shape = new RectangleShape2D();
		shape.Size = _Size;
		SolidCollisionShape.Shape = shape;
		SolidCollisionShape.Position = CollisionShape.Position;
		AddChild(SolidCollisionShape);
	}

	protected override void EnterMushroom()
	{
		foreach (FluidSpring spring in fluidSprings)
		{
			spring.Spread = spring.AcidSpread;
		}
	}

	protected override void ExitMushroom()
	{
		foreach (FluidSpring spring in fluidSprings)
		{
			spring.Spread = spring.BaseSpread;
		}
	}


}
