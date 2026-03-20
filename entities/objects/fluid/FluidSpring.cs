using Godot;
using System;

public partial class FluidSpring : Node2D
{
	[Export] private float damping = 0.4f;
	[Export] private float stiffness = 0.1f;
	[Export] private float spread = 0.2f;
	
	private float velocityY = 0;
	private float targetHeight;

	private FluidSpring leftNeighbor;
	private FluidSpring rightNeighbor;
	
	public override void _Ready()
	{
		targetHeight = Position.Y;
	}

	public void SetupNeighbors(FluidSpring leftNeighbor, FluidSpring rightNeighbor)
	{
		this.rightNeighbor = rightNeighbor;
		this.leftNeighbor = leftNeighbor;
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

		rightNeighbor?.AddExternalForce(spread * (Position.Y - rightNeighbor.Position.Y));
		leftNeighbor?.AddExternalForce(spread * (Position.Y - leftNeighbor.Position.Y));

		Position = Position with { Y = Position.Y + velocityY * (float)delta };
	}

	private float distanceToTarget() => Position.Y - targetHeight;
}
