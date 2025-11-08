using Godot;
using TENamespace;
using TENamespace.health;

namespace TerraEngineer.entities.mobs;

public partial class Entity : CharacterBody2D
{
    [Export] public ComponentManager CM;
    [Export] public CollisionTeam Team;
    
    [Export] public AnimatedSprite2D Sprite;
    [Export] public DirectionX Facing = DirectionX.Right;

    public Vector2 velocity;

    // Used to stop edge cases in which non-garbage collected objects will try to interact with disposed Godot nodes via timed callbacks.
    public bool Dead  = false;

    public override void _Ready()
    {
        if (Facing == DirectionX.Left)
        {
            FlipSprite();
        }
    }

    protected void FlipSprite() => Sprite.FlipH = !Sprite.FlipH;
    
    public void Flip(DirectionX side=DirectionX.None)
    {
        if (side == DirectionX.None || (int)side == -(int)Facing)
        {
            FlipEffect();
        }
    }

    protected virtual void FlipEffect()
    {
        Facing = (DirectionX)(-(int)Facing);
        Sprite.FlipH = !Sprite.FlipH;   
    }

    public virtual void Die()
    {
        if (!Dead)
        {
            CallDeferred(Node.MethodName.QueueFree);    
        }
        Dead = true;
    }
}