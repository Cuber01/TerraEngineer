using Godot;
using System;

public partial class FluidSpring : Node2D
{
	
	[Export] private float damping = 0.4f;
	[Export] private float stiffness = 0.1f;
	
	private float velocityY = 0;
	private float targetHeight;
	
	public override void _Ready()
	{
		targetHeight = Position.Y;
	}

	public void AddExternalForce(float force)
	{
		velocityY += force;
	}

	public override void _Process(double delta)
	{
		if (distanceToTarget() != 0)
		{
			velocityY += -stiffness * distanceToTarget();
		}
		
		velocityY += -(damping * velocityY * (float)delta);	
		Position = Position with { Y = Position.Y + velocityY * (float)delta };
	}

	private float distanceToTarget() => Position.Y - targetHeight;
}
