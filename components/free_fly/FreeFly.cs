using Godot;
using System;
using TENamespace;

public partial class FreeFly : Component
{
    [Export] private float speed = 50.0f;
    [Export] private float acceleration = 0.25f;
    [Export] private float airResistance = 0.1f;

    public void FlyInDirection(Vector2 directionNormal)
    {
        Actor.velocity.X = Mathf.Lerp(Actor.velocity.X, directionNormal.X * speed, acceleration);
        Actor.velocity.Y = Mathf.Lerp(Actor.velocity.Y, directionNormal.Y * speed, acceleration);
    }

    public void FlyToPoint(Vector2 point)
    {
        // float distance =
        //     Mathf.Sqrt(
        //         Mathf.Abs(Actor.GlobalPosition.X - point.X) + 
        //           Mathf.Abs(Actor.GlobalPosition.Y - point.Y)
        //         );
        
        Vector2 direction = (point - Actor.GlobalPosition).Normalized();
        FlyInDirection(direction);
    }
    
    private void updateAirResistance()
    {
        Actor.velocity.X = Mathf.Lerp(Actor.velocity.X, 0f, airResistance);
        Actor.velocity.Y = Mathf.Lerp(Actor.velocity.X, 0f, airResistance);
    }

    public override void Update(float delta) => updateAirResistance();
}
