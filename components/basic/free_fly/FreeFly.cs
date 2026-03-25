using Godot;
using System;
using TENamespace;
using TerraEngineer;
using TerraEngineer.entities.mobs;

public partial class FreeFly : Component
{
    [Export] private float speed = 50.0f;
    [Export] private Vector2 errorMargin = new Vector2(5f, 5f);
    [Export] private float acceleration = 0.25f;
    [Export] private float airResistance = 0.1f;

    private Entity entityActor;

    public override void Init(Node2D actor)
    {
        base.Init(actor);
        if (actor is Entity entity)
        {
            entityActor = entity;
        }
        else
        {
            throw new Exception("FreeFly component requires Entity actor.");
        }
    }

    public void FlyInDirection(Vector2 directionNormal, float dt)
    {
        entityActor.velocity.X = MathT.Lerp(entityActor.velocity.X, directionNormal.X * speed, acceleration, dt);
        entityActor.velocity.Y = MathT.Lerp(entityActor.velocity.Y, directionNormal.Y * speed, acceleration, dt);
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
        entityActor.velocity = MathT.Lerp(entityActor.velocity, Vector2.Zero, airResistance, dt);
    }

    public override void Update(float dt) => updateAirResistance(dt);
}
