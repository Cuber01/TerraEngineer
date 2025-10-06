using Godot;
using System;
using TENamespace;

public partial class FreeFly : Component
{
    [Export] private float speed = 50.0f;
    [Export] private Vector2 errorMargin = new Vector2(5f, 5f);
    [Export] private float acceleration = 0.25f;
    [Export] private float airResistance = 0.1f;

    public void FlyInDirection(Vector2 directionNormal)
    {
        Actor.velocity.X = Mathf.Lerp(Actor.velocity.X, directionNormal.X * speed, acceleration);
        Actor.velocity.Y = Mathf.Lerp(Actor.velocity.Y, directionNormal.Y * speed, acceleration);
    }

    public void FlyToPoint(Vector2 point)
    {
        if (!isAtPoint(Actor.GlobalPosition, point))
        {
            Vector2 direction = (point - Actor.GlobalPosition).Normalized();
            FlyInDirection(direction);
        }
    }

    private bool isAtPoint(Vector2 actorPos, Vector2 pointPosition)
    {
        return (actorPos.X >= pointPosition.X-errorMargin.X && actorPos.Y >= pointPosition.Y-errorMargin.X && actorPos.X <= pointPosition.X + errorMargin.Y && actorPos.Y <= pointPosition.Y + errorMargin.Y);
    }
    
    private void updateAirResistance()
    {
        Actor.velocity.X = Mathf.Lerp(Actor.velocity.X, 0f, airResistance);
        Actor.velocity.Y = Mathf.Lerp(Actor.velocity.Y, 0f, airResistance);
    }

    public override void Update(float delta) => updateAirResistance();
}
