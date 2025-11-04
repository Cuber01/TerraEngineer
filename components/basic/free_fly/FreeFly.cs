using Godot;
using System;
using TENamespace;
using TerraEngineer;

public partial class FreeFly : Component
{
    [Export] private float speed = 50.0f;
    [Export] private Vector2 errorMargin = new Vector2(5f, 5f);
    [Export] private float acceleration = 0.25f;
    [Export] private float airResistance = 0.1f;

    public void FlyInDirection(Vector2 directionNormal, float dt)
    {
        Actor.velocity.X = MathT.Lerp(Actor.velocity.X, directionNormal.X * speed, acceleration, dt);
        Actor.velocity.Y = MathT.Lerp(Actor.velocity.Y, directionNormal.Y * speed, acceleration, dt);
    }

    public void FlyToPoint(Vector2 point, float dt)
    {
        if (!isAtPoint(Actor.GlobalPosition, point))
        {
            Vector2 direction = (point - Actor.GlobalPosition).Normalized();
            FlyInDirection(direction, dt);
        }
    }

    private bool isAtPoint(Vector2 actorPos, Vector2 pointPosition)
    {
        return (actorPos.X >= pointPosition.X-errorMargin.X && actorPos.Y >= pointPosition.Y-errorMargin.Y && 
                actorPos.X <= pointPosition.X + errorMargin.X && actorPos.Y <= pointPosition.Y + errorMargin.Y);
    }
    
    private void updateAirResistance(float dt)
    {
        Actor.velocity = MathT.Lerp(Actor.velocity, Vector2.Zero, airResistance, dt);
    }

    public override void Update(float dt) => updateAirResistance(dt);
}
