using Godot;
using TerraEngineer.entities.mobs;

public partial class MovableBlock : Entity
{
    [Export] private float pushingSpeed = 20;
    
    bool isPushed = false;
    
    public override void _Ready()
    {
        
    }

    public override void _PhysicsProcess(double delta)
    {
        isPushed = false;
        
        checkForPush(Vector2.Left);
        checkForPush(Vector2.Right);
        
        CM.UpdateComponents((float)delta);
        
        if (!isPushed)
        {
            velocity.X = 0;
        }
        
        if (velocity.X == 0 || !TestMove(Transform, velocity*(float)delta))
        {
            Velocity = velocity;
            MoveAndSlide();    
        }
    }
    
    private void checkForPush(Vector2 direction)
    {
        KinematicCollision2D hit = new KinematicCollision2D();
        if (TestMove(GlobalTransform, direction, hit))
        {
            if (hit.GetCollider() is Player player)
            {
                if (Mathf.Abs(hit.GetNormal().Y) == 0)
                {
                    velocity.X = pushingSpeed * hit.GetNormal().X;
                    isPushed = true;
                }
            }
        }
    }
}
