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

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		int move_dir = (int)Input.GetAxis("ui_left", "ui_right");
		if (move_dir != 0)
		{
			facing = move_dir;
		}
		
		if (Input.IsActionJustPressed("dash"))
		{
			componentManager.GetComponent<Dash>().AttemptDash(facing);
		}
	
		if (Input.IsActionJustPressed("jump"))
		{
			componentManager.GetComponent<Jump>().AttemptJump();
		}
		
		componentManager.GetComponent<Move>().Walk(move_dir);
		if (!IsOnFloor())
		{
			componentManager.GetComponent<Gravity>().ApplyGravity((float)delta);
		}
		else
		{
			Mathf.Clamp(velocity.Y, 0, 999999999);
		}
		
		componentManager.UpdateComponents((float)delta);

		Velocity = velocity;
		MoveAndSlide();
	}
}
