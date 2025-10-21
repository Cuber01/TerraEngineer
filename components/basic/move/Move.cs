using Godot;
using TerraEngineer;
using TerraEngineer.entities.mobs;

namespace TENamespace;

public partial class Move : Component
{
	[Export] private float speed = 50.0f;
	[Export] private float acceleration = 0.25f;
	[Export] private float friction = 0.1f;

	[Export] public bool FrictionEnabledX = true;
	[Export] public bool FrictionEnabledY = false;
	
	public override void Update(float delta) => updateFriction(FrictionEnabledX, FrictionEnabledY);
	
	public void Walk(DirectionX direction)
	{
		Actor.velocity.X = Mathf.Lerp(Actor.velocity.X, (int)direction * speed, acceleration);
	}

	public void Walk4(Vector2 direction)
	{
		Actor.velocity = MathT.Lerp(Actor.velocity, direction * speed, acceleration);
	}

	private void updateFriction(bool x, bool y)
	{
		if(x) Actor.velocity.X = Mathf.Lerp(Actor.velocity.X, 0f, friction);	
		if(y) Actor.velocity.Y = Mathf.Lerp(Actor.velocity.Y, 0f, friction);
	}

}






