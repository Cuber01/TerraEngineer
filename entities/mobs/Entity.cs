#define TOOLS

using Godot;
using System;
using TENamespace;
using TENamespace.health;

namespace TerraEngineer.entities.mobs;


[Tool]
public partial class Entity : CharacterBody2D
{
    [Export] public ComponentManager CM;
    [Export] public CollisionTeam Team;
    [Export] public bool GodMode = false;
    [Export] public AnimatedSprite2D Sprite;
    
    [Export] public DirectionX ExportedFacing
    {
        get => Facing;
        set
        {
            Flip(value);
        }
    }
    public DirectionX Facing = DirectionX.Right;
    public Vector2 velocity;
    
    // Used to stop edge cases in which non-garbage collected objects will try to interact with disposed Godot nodes via timed callbacks.
    public bool Dead  = false;
    
    public override void _Ready()
    {
        #if TOOLS
        if (Engine.IsEditorHint())
        {
            SetPhysicsProcess(false);
            SetProcess(false);
            return;
        }
        #endif
    }

    protected void MakeShaderUnique()
    {
        Material mat = (Material)GetNode<AnimatedSprite2D>(Names.Node.AnimatedSprite2D).Material.Duplicate(true);
        GetNode<AnimatedSprite2D>(Names.Node.AnimatedSprite2D).Material = mat;
    }

    protected void FlipSprite() => Sprite.FlipH = !Sprite.FlipH;
    
    public void Flip(DirectionX side=DirectionX.None)
    {
        if ((side == DirectionX.None || (int)side == -(int)Facing) && Sprite != null)
        {
            #if TOOLS
            if(Engine.IsEditorHint())
                GD.Print("Flipping to "  + side);
            #endif
            
            FlipEffect();
        }
        #if TOOLS
        else if(Engine.IsEditorHint())
            GD.Print("Failed flipping to "  + side);
        #endif
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

    protected void HandleMove()
    {
        Velocity = new Vector2(MathF.Truncate(velocity.X), MathF.Truncate(velocity.Y));
        MoveAndSlide();
    }
}