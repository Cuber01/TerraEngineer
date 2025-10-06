using Godot;
using System;
using TENamespace;
using TerraEngineer.entities.mobs;

public partial class Player : Mob
{

	public override void _PhysicsProcess(double delta)
	{
		CM.GetComponent<Gravity>().UpdateGravity((float)delta);
		
		DirectionX moveDir = (DirectionX)(int)Input.GetAxis("ui_left", "ui_right");
		if (moveDir != 0)
		{
			Facing = moveDir;
		}
		
		if (Input.IsActionJustPressed("dash"))
		{
			CM.GetComponent<Dash>().AttemptDash(Facing);
		}
		
		CM.GetComponent<Move>().Walk(moveDir);
		CM.GetComponent<Move>().UpdateFriction();
		
		if (Input.IsActionJustPressed("jump"))
		{
			CM.GetComponent<Jump>().AttemptJump();
		}
		
		CM.UpdateComponents((float)delta);

		Velocity = velocity;
		MoveAndSlide();
	}
}
