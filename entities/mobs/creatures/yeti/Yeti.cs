using Godot;
using System;
using TENamespace.projectile_builder;
using TerraEngineer.entities.mobs.creatures;

[Tool]
public partial class Yeti : Creature
{
	private float spawnTime = 3f;
	
	public override void Init()
	{
		TimerManager.Schedule(spawnTime, this, spawnSnowball);
	}

	private void spawnSnowball(ITimer timer)
	{
		CM.GetComponent<ProjectileSpawner>()
			.Start()
			.SetFacing(Facing.Opposite())
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
	}
}
