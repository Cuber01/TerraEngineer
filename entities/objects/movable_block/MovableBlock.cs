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
    private float pushingSpeed = 100;
    private bool isPushed = false;
    private float pushDirection = 0;

    public override void Update(float delta)
    {
        isPushed = false;
        checkIfPushed(Vector2.Left);
        checkIfPushed(Vector2.Right);
    
        CM.UpdateComponents(delta);

        if (isPushed)
        {
            velocity.X = pushingSpeed * pushDirection;
        } 
        else if (StopCondition())
        {
            velocity.X = 0;
        }

        Velocity = velocity;
        MoveAndSlide();
        velocity = Velocity; // Sync back so Gravity knows we hit the floor
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
                    isPushed = true;
                    pushDirection = hit.GetNormal().X;
                    pushingSpeed = Math.Abs(player.velocity.X);
                }
            }
        }
    }
    
    protected virtual bool StopCondition() => !isPushed;

    public override void Disable()
    {
        base.Disable();
        velocity.X = 0;
    }
}
