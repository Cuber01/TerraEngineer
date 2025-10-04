using Godot;

namespace TENamespace;

public partial class Move : Component
{
	[Export] private float speed = 300.0f;
	[Export] private float acceleration = 0.25f;
	[Export] private float friction = 0.1f;

	public void Walk(float direction)
	{
		Actor.velocity.X = Mathf.Lerp(Actor.velocity.X, direction * speed, acceleration);
	}

	public void ApplyFriction()
	{
		Actor.velocity.X = Mathf.Lerp(Actor.velocity.X, 0f, friction);
	}
	
}






