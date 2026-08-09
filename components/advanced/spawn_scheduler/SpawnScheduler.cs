using Godot;
using System;
using TENamespace;
using TENamespace.basic.builders;
using TerraEngineer;

public partial class SpawnScheduler : Node2D
{
	[ExportGroup("External")]
	[Export] private ReferenceRect spawnSpace;
	[Export] private float spawnChance = 1.0f;
	[Export] private float spawnInterval = 1.0f;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TimerManager.Schedule(spawnInterval, this, tick);
	}

	private void tick(ITimer _)
	{
		TimerManager.Schedule(spawnInterval, this, tick);
		if (MathT.RandomFloat(0.0f, 1.0f) <= spawnChance)
		{
			Spawn(MathT.RandomPositionInRect(GlobalPosition, spawnSpace.Size));	
		}
	}

	protected virtual void Spawn(Vector2 position)
	{
		throw new NotImplementedException("Needs to be overriden.");
	}
}
