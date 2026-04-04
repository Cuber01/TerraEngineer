using Godot;

namespace TerraEngineer.entities.objects.movable_block;

public partial class StickyBlock : MovableBlock
{
    private bool isLatched = false;
    private Player latchedPlayer = null;

    protected override void HandleVelocity(float delta)
    {
        if (IsPushed && !isLatched)
        {
            isLatched = true;
        }

        if (isLatched)
        {
            if (latchedPlayer == null || Mathf.Abs(latchedPlayer.velocity.Y) > 0.1f)
            {
                isLatched = false;
                latchedPlayer = null;
                velocity.X = 0;
                return;
            }

            if((PushDirection < 0 && ((latchedPlayer.GlobalPosition.X + latchedPlayer.velocity.X) - GlobalPosition.X) > 0 )
               || (PushDirection > 0 && ((latchedPlayer.GlobalPosition.X + latchedPlayer.velocity.X) - GlobalPosition.X) < 0) )
            {
                velocity.X = latchedPlayer.velocity.X;
            }
            else
            {
                // All of these are necessary to workaround multiple behaviors of the physics engine
                latchedPlayer.velocity.X = 0;
                latchedPlayer.Velocity = new Vector2(0,latchedPlayer.Velocity.Y);
                Position = new  Vector2(0, Position.Y);
                velocity.X = 0;
            }
        }
        else
        {
            velocity.X = Mathf.MoveToward(velocity.X, 0, PushingSpeed * 0.2f);
        }
    }

    protected override void CheckIfPushed(Vector2 direction)
    {
        KinematicCollision2D hit = new KinematicCollision2D();
        if (TestMove(GlobalTransform, direction * 1.1f, hit))
        {
            if (hit.GetCollider() is Player p)
            {
                IsPushed = true;
                PushDirection = hit.GetNormal().X;
                latchedPlayer = p; 
            }
        }
    }
}