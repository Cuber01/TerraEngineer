using Godot;
using System;
using TENamespace.projectile_builder;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.mobs.creatures;

[Tool]
public partial class Yeti : Creature
{
	[Export] private Node2D spawnPos;
	private float spawnTime = 3f;
	
	public override void Init()
	{
		// TODO no idea why yeti fails to flip without this
		if (Facing == DirectionX.Left)
		{
			Flip();
			Facing = DirectionX.Left;
		}
			
		TimerManager.Schedule(spawnTime, this, spawnSnowball);
	}

	private void spawnSnowball(ITimer timer)
	{
		CM.GetComponent<ProjectileSpawner>()
			.Start()
			.SetFacing(Facing)
			.AddToGame()
			.Build();
		
		TimerManager.Schedule(spawnTime, this, spawnSnowball);
	}

	public override void _PhysicsProcess(double delta)
	{
		#if TOOLS
		if (Engine.IsEditorHint())
			return;
		#endif
		
		CM.UpdateComponents((float)delta);
		HandleMove();
	}
	
	protected override void FlipEffect()
	{
		base.FlipEffect();
		spawnPos.Position = new Vector2(-spawnPos.Position.X, spawnPos.Position.Y);
	}
}
