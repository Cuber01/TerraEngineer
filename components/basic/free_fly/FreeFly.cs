using Godot;
using System;
using TENamespace;
using TerraEngineer;
using TerraEngineer.entities.mobs;

public partial class FreeFly : Component
{
    [ExportGroup("External")]
    
    [Export] protected float speed = 50.0f;
    [Export] private Vector2 errorMargin = new Vector2(5f, 5f);
    [Export] protected float acceleration = 0.25f;
    [Export] private float airResistance = 0.1f;

    protected Entity EntityActor;

    public override void Init(Node2D actor)
    {
        base.Init(actor);
        if (actor is Entity entity)
        {
            EntityActor = entity;
        }
        else
        {
            throw new Exception("FreeFly component requires Entity actor.");
        }
    }

    public void FlyInDirection(Vector2 directionNormal, float dt)
    {
        EntityActor.velocity.X = MathT.Lerp(EntityActor.velocity.X, directionNormal.X * speed, acceleration, dt);
        EntityActor.velocity.Y = MathT.Lerp(EntityActor.velocity.Y, directionNormal.Y * speed, acceleration, dt);
    }

    public void FlyToPoint(Vector2 point, float dt, Action<Vector2, float> flyStyle = null)
    {
        if (!isAtPoint(Actor.GlobalPosition, point))
        {
            Vector2 direction = (point - Actor.GlobalPosition).Normalized();
            if (flyStyle == null)
            {
                FlyInDirection(direction, dt);    
            }
            else
            {
                flyStyle(direction, dt);
            }
            
        }
    }

    private bool isAtPoint(Vector2 actorPos, Vector2 pointPosition)
    {
        return (actorPos.X >= pointPosition.X-errorMargin.X && actorPos.Y >= pointPosition.Y-errorMargin.Y && 
                actorPos.X <= pointPosition.X + errorMargin.X && actorPos.Y <= pointPosition.Y + errorMargin.Y);
    }
    
    private void updateAirResistance(float dt)
    {
        EntityActor.velocity = MathT.Lerp(EntityActor.velocity, Vector2.Zero, airResistance, dt);
    }

    public void MultiplyAcceleration(float by)
    {
        acceleration *= by;
    }

    public override void Update(float dt) => updateAirResistance(dt);
}
