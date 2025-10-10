using Godot;
using TENamespace;
using TENamespace.health;

namespace TerraEngineer.entities.mobs;

public partial class Mob : CharacterBody2D
{
    [Export] public ComponentManager CM;
    [Export] public CollisionTeam Team;
    
    [Export] private AnimatedSprite2D sprite;
    [Export] public DirectionX Facing = DirectionX.Right;
    public Vector2 velocity;

    protected void Flip()
    {
        Facing = (DirectionX)(-(int)Facing);
        sprite.FlipH = !sprite.FlipH;
    }

    public virtual void Die()
    {
        QueueFree();
    }
}