using Godot;
using TENamespace.basic.builders;
using TerraEngineer.entities.projectiles;

namespace TENamespace.projectile_builder;

public partial class ProjectileSpawner : Spawner<Projectile, ProjectileSpawner>
{
    public ProjectileSpawner SetDirectionNormal(Vector2 direction)
    {   
        Instance.DirectionNormal = direction;
        return this;
    }
}