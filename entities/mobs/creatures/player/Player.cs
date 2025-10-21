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
			CM.GetComponent<GunHandle>().Shoot(getShootDirection(), false);
		}

		if (Input.IsActionJustPressed("weapon_0"))
			CM.GetComponent<GunHandle>().ChangeWeapon(0);
		else if (Input.IsActionJustPressed("weapon_1"))
			CM.GetComponent<GunHandle>().ChangeWeapon(1);
		else if (Input.IsActionJustPressed("weapon_2"))
			CM.GetComponent<GunHandle>().ChangeWeapon(2);
		else if (Input.IsActionJustPressed("weapon_3"))
			CM.GetComponent<GunHandle>().ChangeWeapon(3);
		else if (Input.IsActionJustPressed("weapon_next"))
			CM.GetComponent<GunHandle>().ChangeToNextWeapon();
		
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

	private Direction4 getShootDirection()
	{
		Vector2 vector = Input.GetVector("ui_left", "ui_right", 
										 "ui_down", "ui_up");
		if (vector.Y > 0) return Direction4.Up;
		if (vector.Y < 0) return Direction4.Down;
		if (vector.X > 0) return Direction4.Right;
		if (vector.X < 0) return Direction4.Left;

		return (Direction4)(int)Facing;
	}
}


