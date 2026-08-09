using Godot;
using System;
using TENamespace;
using TerraEngineer.entities.projectiles;

public partial class Snowball : Projectile
{
	public override void _Ready()
	{
		base._Ready();
		InitSpriteWrapper();	
	}
	
	public override void _PhysicsProcess(double delta)
	{
		
		CM.GetComponent<Move>().Walk(Facing, (float)delta);
		CM.UpdateComponents((float)delta);
		
		HandleMove();
		FlipIfHitWall();
	}

	protected override void FlipEffect()
	{
		base.FlipEffect();
		velocity = -velocity;
	}
}
