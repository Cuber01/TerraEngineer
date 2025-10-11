using Godot;
using System;
using TENamespace;
using TENamespace.projectile_builder;
using TerraEngineer.entities.mobs;

public partial class Player : Mob
{

	public override void _PhysicsProcess(double delta)
	{
		DirectionX moveDir = (DirectionX)(int)Input.GetAxis("ui_left", "ui_right");
		if (moveDir != 0)
		{
			Facing = moveDir;
		}
		
		if (Input.IsActionJustPressed("dash"))
		{
			CM.GetComponent<Dash>().AttemptDash(Facing);
		}
		
		if (Input.IsActionJustPressed("debug"))
		{
			CM.GetComponent<Gun>().Shoot(Directions4.Up);
		}
		
		CM.GetComponent<Move>().Walk(moveDir);
		CM.UpdateComponents((float)delta);
		
		if (Input.IsActionJustPressed("jump"))
		{
			CM.GetComponent<Jump>().AttemptJump();
		}
		
		CM.UpdateComponents((float)delta);

		Velocity = velocity;
		MoveAndSlide();
	}
}
