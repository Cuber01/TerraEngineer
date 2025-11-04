using Godot;

namespace TerraEngineer.entities.projectiles;

public partial class BasicBullet : Projectile
{
    public override void _PhysicsProcess(double delta)
    {
        CM.UpdateComponents((float)delta);
        
        CM.GetComponent<FreeFly>().FlyInDirection(DirectionNormal, (float)delta);
        
        Velocity = velocity;
        MoveAndSlide();
    }
}