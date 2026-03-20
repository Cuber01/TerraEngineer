using Godot;
using System;

public partial class FluidSpring : Node2D
{
	
	[Export] private float damping = 0.2f;
	[Export] private float stiffness = 0.1f;
	
	private float velocityY = 0;
	private float targetHeight;
	
	public override void _Ready()
	{
		targetHeight = Position.Y;
	}

	public override void _Process(double delta)
	{
		velocityY += -stiffness * distanceToTarget();
		velocityY += (velocityY >= 0 ? 1 : -1) * (damping * velocityY);
	}

	private float distanceToTarget() => Position.Y - targetHeight;
}
