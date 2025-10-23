using Godot;
using TENamespace.basic.builders;
using TerraEngineer.entities.projectiles;

namespace TENamespace.projectile_builder;

public partial class ProjectileSpawner : Spawner
{
    public Projectile Build(Vector2 position, Vector2 directionNormal, float rotationDegrees)
    {
        Projectile instance = (Projectile)Scene.Instantiate();
        instance.DirectionNormal = directionNormal;
        instance.GlobalPosition = position;
        instance.RotationDegrees = rotationDegrees;
        
        return instance;
    }
}