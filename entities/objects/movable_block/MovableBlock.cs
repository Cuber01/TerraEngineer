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
    

    public override void Update(float delta)
    {
        IsPushed = false;
        checkIfPushed(Vector2.Left);
        checkIfPushed(Vector2.Right);

        HandleVelocity();
        
        CM.UpdateComponents(delta);
        
        HandleMove();
    }
    
    private void checkIfPushed(Vector2 direction)
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

    protected virtual void HandleVelocity()
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
