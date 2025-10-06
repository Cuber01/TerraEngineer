using Godot;
using System;
using TENamespace;

public partial class FreeFly : Component
{
    [Export] private float speed = 50.0f;
    [Export] private float acceleration = 0.25f;
    [Export] private float airResistance = 0.1f;

    // public void FlyInDirection(DirectionX direction)
    // {
    //     Actor.velocity.X = Mathf.Lerp(Actor.velocity.X, (int)direction * speed, acceleration);
    // }

    public void FlyToPoint(Vector2 point)
    {
        
    }
    
    public void UpdateAirResistance()
    {
        // if (Actor.velocity.X != 0)
        // {
        //     Actor.velocity.X = Mathf.Lerp(Actor.velocity.X, 0f, friction);	
        // }
    }

    public override void Update(float delta) => UpdateAirResistance();
}
