#define TOOLS

using Godot;
using System;
using TENamespace;
using TerraEngineer.game.sprite;

namespace TerraEngineer.entities.mobs;

[Tool]
public partial class Entity : CharacterBody2D
{
    [Signal] public delegate void DiedEventHandler();
    
    [Export] public ComponentManager CM;
    [Export] public CollisionTeam Team;
    [Export] public bool GodMode = false;
    [Export] public Node2D Sprite;

    [Export] public SpriteWrapper ExportedSpriteWrapper
    {
        get => SpriteWrapper;
        set
        {
            SpriteWrapper = value;
            #if TOOLS
            if(Engine.IsEditorHint())
                SpriteWrapper.Init(Sprite);
            #endif
        }
    }
    public SpriteWrapper SpriteWrapper;
    
    [Export] public DirectionX ExportedFacing
    {
        get => Facing;
        set => Flip(value);
    }
    public DirectionX Facing = DirectionX.Right;
    public Vector2 velocity;
    
    // Used to stop edge cases in which non-garbage collected objects will try to interact with disposed Godot nodes via timed callbacks.
    public bool Dead = false;
    
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

    protected void FlipSprite()
    {
        SpriteWrapper.Flip();
    }
    
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
        SpriteWrapper.Flip();
    }

    public virtual void Die()
    {
        if (!Dead)
        {
            EmitSignal(SignalName.Died);
            CallDeferred(Node.MethodName.QueueFree);    
        }
        Dead = true;
    }
    
    public virtual void HandleMove()
    {
        Velocity = velocity;
        MoveAndSlide();
        //velocity = Velocity;
    }

    protected void InitSpriteWrapper()
    {
        ExportedSpriteWrapper = (SpriteWrapper)ExportedSpriteWrapper.Duplicate();
        SpriteWrapper.Init(Sprite);
    }

}