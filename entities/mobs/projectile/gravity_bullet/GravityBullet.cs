namespace TerraEngineer.entities.projectiles.gravity_bullet;

public partial class GravityBullet : Projectile
{
    public override void _PhysicsProcess(double delta)
    {
        CM.UpdateComponents((float)delta);
        HandleMove();
    }

    protected override void OnDeflect()
    {
        DirectionNormal = -DirectionNormal;
        velocity = -velocity;
        ReverseTeams();
    }
}