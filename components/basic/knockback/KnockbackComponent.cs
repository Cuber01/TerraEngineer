using System;
using Godot;

namespace TENamespace.basic;

public partial class KnockbackComponent : Component
{
    [Export] private Vector2 knockbackFactor = Vector2.One;
    
    public void ApplyKnockback(Vector2 fromPosition, float force)
    {
        Vector2 dist = new(fromPosition.X - Actor.GlobalPosition.X,  (fromPosition.Y - Actor.GlobalPosition.Y));
        Vector2 dir = -dist.Normalized();
        Actor.velocity += dir * force * knockbackFactor;
    }
}