namespace TerraEngineer.entities.projectiles.gravity_bullet;

public partial class GravityBullet : Projectile
{
    public override void _PhysicsProcess(double delta)
    {
        CM.UpdateComponents((float)delta);
        HandleMove();
    }

    protected override void OnDeflected()
    {
        DirectionNormal = -DirectionNormal;
        velocity = -velocity;
        ReverseCollision();
    }
}