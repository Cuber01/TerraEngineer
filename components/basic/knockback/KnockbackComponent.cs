using Godot;
using System;
using TerraEngineer.entities.mobs;

namespace TENamespace.basic;

public partial class KnockbackComponent : Component
{
    [Export] private Vector2 knockbackFactor = new(1.0f, 0.5f);
    
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
            throw new Exception("KnockbackComponent requires Entity actor.");
        }
    }

    public void ApplyKnockback(Vector2 fromPosition, float force)
    {
        Vector2 dist = new(fromPosition.X - Actor.GlobalPosition.X,  (fromPosition.Y - Actor.GlobalPosition.Y));
        Vector2 dir = -dist.Normalized();
        entityActor.velocity += dir * force * knockbackFactor;
    }
}