using Godot;
using System;
using System.Numerics;
using TerraEngineer;
using TerraEngineer.entities;
using TerraEngineer.entities.mobs;
using Vector2 = Godot.Vector2;

public partial class MovableBlock : Terraformable
{
    private const float TestMoveBuffer = 1.1f;
    private const float BaseSlideSpeed = 100;
    protected const float VelocityNeededToPush = 40f;
    
    protected float PushingSpeed = 100;
    protected bool IsPushed = false;
    protected float PushDirection = 0;
    
    private bool resolvingEdgeCase = false;

    public override void Update(float delta)
    {
        IsPushed = false;
        
        // Can't be pushed while falling
        if (velocity.Y > 0)
        {
            if (!resolvingEdgeCase)
            {
                velocity.X = 0;
            }
            else
            {
                resolvingEdgeCase = false;
            }
                
        }
        else
        {
            CheckIfPushed(Vector2.Left);
            CheckIfPushed(Vector2.Right);
        }
        
        HandleVelocity(delta);

        ResolveDownwardPlayerCollision();
        
        CM.UpdateComponents(delta);
        
        HandleMove();
    }
    
    protected virtual void CheckIfPushed(Vector2 direction)
    {
        KinematicCollision2D hit = new KinematicCollision2D();
        if (TestMove(GlobalTransform, direction * TestMoveBuffer, hit))
        {
            if (hit.GetCollider() is Player player)
            {
                if (Mathf.Abs(hit.GetNormal().Y) == 0)
                {
                    IsPushed = true;
                    PushDirection = hit.GetNormal().X;
                    PushingSpeed = Math.Abs(player.velocity.X);
                }
            }
        }
    }
    
    // Edgecase when a box falls on the player an the player jumps
    protected void ResolveDownwardPlayerCollision()
    {
        KinematicCollision2D hit = new KinematicCollision2D();
        if (TestMove(GlobalTransform, Vector2.Down * TestMoveBuffer, hit))
        {
            if (hit.GetCollider() is Player)
            {
                velocity = velocity with { X = 10 };
                resolvingEdgeCase = true;
            }
        }
    }

    protected virtual void HandleVelocity(float delta)
    {
        if (IsPushed && PushingSpeed > VelocityNeededToPush)
        {
            velocity.X = PushingSpeed * PushDirection;
        } 
        else if (!IsPushed)
        {
            velocity.X = Mathf.MoveToward(velocity.X, 0, PushingSpeed * 0.2f);
        }
    }

    public override void Disable()
    {
        base.Disable();
        velocity.X = 0;
    }
}
