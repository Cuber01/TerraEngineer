using Godot;

namespace TerraEngineer.entities.objects.movable_block;

public partial class StickyBlock : Terraformable
{
    private bool isPulled = false;
    private float pullDirection = 0;
    private float pullingSpeed = 0;
    
    public override void Update(float delta)
    {
        isPulled = false;
        
        checkForPulled(Vector2.Left);
        checkForPulled(Vector2.Right);
        
        CM.UpdateComponents((float)delta);

        if (isPulled)
        {
            velocity.X = pullingSpeed * pullDirection;
        } else if (!isPulled)
        {
            velocity.X = 0;
        }
        
        if (velocity.X == 0 || !TestMove(Transform, velocity*(float)delta))
        {
            Velocity = velocity;
            MoveAndSlide();    
        }
    }
    
    private void checkForPulled(Vector2 direction)
    {
        KinematicCollision2D hit = new KinematicCollision2D();
        if (TestMove(GlobalTransform, direction, hit))
        {
            if (hit.GetCollider() is Player player)
            {
                if (Mathf.Abs(hit.GetNormal().Y) == 0)
                {
                    isPulled = true;
                    pullDirection = -hit.GetNormal().X;
                }
            }
        }
    }

    
}