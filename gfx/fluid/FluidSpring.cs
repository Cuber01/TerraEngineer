using Godot;
using System;
using TerraEngineer;

[Tool]
public partial class FluidSpring : Node2D
{
	[Export] private float damping = 0.7f;
	[Export] private float stiffness = 0.1f;
	[Export] private float spread = 0.6f;
	[Export] private float spreadDamping = 0.9995f;
	private const float VelocityThreshold = 0.1f;
	private const float PositionThreshold = 0.01f;
	
	// Damping is increased for coastal springs to simulate waves crushing against the coast
	[Export] private float coastalDamping = 0.9f;
	[Export] private float coastalStiffness = 0.2f;
	
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

		if (leftNeighbor == null || rightNeighbor == null)
			damping = coastalDamping;
	}

	public void AddExternalForce(float force, float newSpread)
	{
		spread = newSpread;
		velocityY += force;
	}

	public override void _PhysicsProcess(double delta)
	{
		#if TOOLS
		if(Engine.IsEditorHint())
			return;
		#endif
		
		float dist = distanceToTarget();
		
		if (Mathf.Abs(dist) < PositionThreshold && Mathf.Abs(velocityY) < VelocityThreshold)
		{
			Position = Position with { Y = targetHeight };
			velocityY = 0;
			return;
		}
		
		velocityY -= stiffness * distanceToTarget();
		velocityY += -(damping * velocityY * (float)delta);
		
		leftNeighbor?.AddExternalForce(spread * (Position.Y - leftNeighbor.Position.Y), spread * spreadDamping);
		rightNeighbor?.AddExternalForce(spread * (Position.Y - rightNeighbor.Position.Y), spread * spreadDamping);
		
		Position = Position with { Y = Position.Y + velocityY * (float)delta };
	}

	private float distanceToTarget() => Position.Y - targetHeight;
}
