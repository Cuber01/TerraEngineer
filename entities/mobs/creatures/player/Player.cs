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
		
		if (Input.IsActionJustPressed("attack"))
		{
			CM.GetComponent<Gun>().Shoot(getShootDirection(), true);
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

	private Directions4 getShootDirection()
	{
		Vector2 vector = Input.GetVector("ui_left", "ui_right", 
										 "ui_down", "ui_up");
		if (vector.Y > 0) return Directions4.Up;
		if (vector.Y < 0) return Directions4.Down;
		if (vector.X > 0) return Directions4.Right;
		if (vector.X < 0) return Directions4.Left;

		return (Directions4)(int)Facing;
	}
}


