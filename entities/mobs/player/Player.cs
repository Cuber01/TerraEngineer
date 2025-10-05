using Godot;
using System;
using TENamespace;
using TerraEngineer.entities.mobs;

public partial class Player : Mob
{

	
	private int facing = 1;
	
	public override void _Ready()
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		CM.GetComponent<Gravity>().UpdateGravity((float)delta);
		
		int moveDir = (int)Input.GetAxis("ui_left", "ui_right");
		if (moveDir != 0)
		{
			facing = moveDir;
		}
		
		if (Input.IsActionJustPressed("dash"))
		{
			CM.GetComponent<Dash>().AttemptDash(facing);
		}
		
		CM.GetComponent<Move>().Walk(moveDir);

		
		if (Input.IsActionJustPressed("jump"))
		{
			CM.GetComponent<Jump>().AttemptJump();
		}
		
		CM.UpdateComponents((float)delta);

		Velocity = velocity;
		MoveAndSlide();
	}
}
