using Godot;
using System;
using TENamespace;
using TerraEngineer.entities.mobs;

public partial class Player : Mob
{
	[Export] ComponentManager componentManager;
	private int facing = 1;
	
	public override void _Ready()
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!IsOnFloor())
		{
			componentManager.GetComponent<Gravity>().ApplyGravity((float)delta);
		}
		else
		{
			velocity.Y = Mathf.Clamp(velocity.Y, -999999999, 0);
		}
		
		int moveDir = (int)Input.GetAxis("ui_left", "ui_right");
		if (moveDir != 0)
		{
			facing = moveDir;
		}
		
		if (Input.IsActionJustPressed("dash"))
		{
			componentManager.GetComponent<Dash>().AttemptDash(facing);
		}
		
		componentManager.GetComponent<Move>().Walk(moveDir);

		
		if (Input.IsActionJustPressed("jump"))
		{
			componentManager.GetComponent<Jump>().AttemptJump();
		}
		
		componentManager.UpdateComponents((float)delta);

		Velocity = velocity;
		MoveAndSlide();
	}
}
