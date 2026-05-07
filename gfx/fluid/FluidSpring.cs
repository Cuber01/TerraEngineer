using Godot;

[Tool]
public partial class FluidSpring : Node2D
{
	private float damping = 0.7f;
	private float stiffness = 0.1f;
	public float Spread = -1; // = BaseSpread in _ready
	private const float SpreadDamping = 0.9995f;
	public readonly float AcidSpread = 0.8f;
	public readonly float BaseSpread = 0.6f;
	private const float VelocityThreshold = 0.1f;
	private const float PositionThreshold = 0.01f;
	
	// Damping is increased for coastal springs to simulate waves crushing against the coast
	private const float CoastalDamping = 0.9f;
	private const float CoastalStiffness = 0.6f;
	
	private float velocityY = 0;
	private float targetHeight;

	private FluidSpring leftNeighbor;
	private FluidSpring rightNeighbor;
	
	public override void _Ready()
	{
		Spread = BaseSpread;
		targetHeight = Position.Y;
	}

	public void SetupNeighbors(FluidSpring leftNeighbor, FluidSpring rightNeighbor)
	{
		this.rightNeighbor = rightNeighbor;
		this.leftNeighbor = leftNeighbor;

		if (leftNeighbor == null || rightNeighbor == null)
		{
			damping = CoastalDamping;	
			stiffness = CoastalStiffness; 
		}
	}

	public void AddExternalForce(float force, float newSpread)
	{
		Spread = newSpread;
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
		
		leftNeighbor?.AddExternalForce(Spread * (Position.Y - leftNeighbor.Position.Y), Spread * SpreadDamping);
		rightNeighbor?.AddExternalForce(Spread * (Position.Y - rightNeighbor.Position.Y), Spread * SpreadDamping);
		
		Position = Position with { Y = Position.Y + velocityY * (float)delta };
	}

	private float distanceToTarget() => Position.Y - targetHeight;
}
