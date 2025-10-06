using Godot;
using TerraEngineer.entities.mobs;

namespace TENamespace;

public partial class Move : Component
{
	[Export] private float speed = 300.0f;
	[Export] private float acceleration = 0.25f;
	[Export] private float friction = 0.1f;

	public void Walk(DirectionX direction)
	{
		Actor.velocity.X = Mathf.Lerp(Actor.velocity.X, (int)direction * speed, acceleration);
	}

	public void UpdateFriction()
	{
		if (Actor.velocity.X != 0)
		{
			Actor.velocity.X = Mathf.Lerp(Actor.velocity.X, 0f, friction);	
		}
	}
	
}






