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
    [Export] public bool GodMode = false;
    [Export] public Node2D Sprite;
    [Export] public float Weight = 64f; // For fluids

    [Export] public SpriteWrapper ExportedSpriteWrapper
    {
        get => SpriteWrapper;
        set
        {
            SpriteWrapper = value;
            #if TOOLS
            if(Engine.IsEditorHint() && Sprite != null && SpriteWrapper != null)
                InitSpriteWrapper();
            #endif
        }
    }
    public SpriteWrapper SpriteWrapper;
    
    [Export] public DirectionX ExportedFacing
    {
        get => Facing;
        set
        {
            if(Engine.IsEditorHint())
                Flip(value);
        }
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
    
    #region flipping
    public void Flip() => Flip(DirectionX.None);
    
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
        Facing = Facing.Opposite();
        SpriteWrapper.Flip();
    }
    
    protected void FlipIfHitWall() => DoActionIfHitWall(Flip);

    protected void DoActionIfHitWall(Action action)
    {
        for(int i = 0; i < GetSlideCollisionCount(); i++)
        {
            Vector2 normal = GetSlideCollision(i).GetNormal();
            if (normal == new Vector2(-(int)Facing, 0))
            {
                action.Invoke();
                return;
            }
        }
    }
    #endregion

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
    
    public virtual void FellIntoFluid(Fluid fluid) {}

    protected void InitSpriteWrapper()
    {
        SpriteWrapper = (SpriteWrapper)ExportedSpriteWrapper.Duplicate();
        SpriteWrapper.Init(Sprite);
    }

}