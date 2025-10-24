using Godot;
using TerraEngineer;
using TerraEngineer.entities.mobs;

namespace TENamespace;

public partial class Move : Component
{
	[Export] private float speed = 50.0f;
	[Export] private float acceleration = 0.25f;
	[Export] private float friction = 0.1f;
	[Export] private float airResistance = 0.1f;

	[Export] public bool FrictionEnabledX = true;
	[Export] public bool FrictionEnabledY = false;
	
	[Export] private float errorMargin = 1f;
	
	public override void Update(float delta) => updateFriction(FrictionEnabledX, FrictionEnabledY);
	
	public void Walk(DirectionX direction, float speedModifier=1f)
	{
		Actor.velocity.X = Mathf.Lerp(Actor.velocity.X, (int)direction * speed * speedModifier, acceleration);
	}

	public void Walk4(Vector2 direction)
	{
		Actor.velocity = MathT.Lerp(Actor.velocity, direction * speed, acceleration);
	}

	public void WalkToPoint(float pointX, float speedModifier=1f)
	{
		if (!isAtPoint(Actor.GlobalPosition.X, pointX))
		{
			Walk(pointX > Actor.GlobalPosition.X ? DirectionX.Right: DirectionX.Left,
				speedModifier);
		}	
	}

	private bool isAtPoint(float posX, float pointX)
	{
		return (posX >= pointX - errorMargin && posX <= pointX + errorMargin);
	}

	private void updateFriction(bool x, bool y)
	{
		float amount;
		if (Actor.IsOnFloor())
		{
			amount = friction;
		}
		else
		{
			amount = airResistance;
		}
		
		if (x) Actor.velocity.X = Mathf.Lerp(Actor.velocity.X, 0f, amount);	
		if (y) Actor.velocity.Y = Mathf.Lerp(Actor.velocity.Y, 0f, amount);
	}

}






