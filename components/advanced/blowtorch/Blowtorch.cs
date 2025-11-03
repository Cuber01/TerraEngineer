using Godot;
using TENamespace.advanced.main_gun_wrapper;
using TENamespace.projectile_builder;
using TerraEngineer.entities.projectiles;

namespace TENamespace.advanced.blowtorch;

public partial class Blowtorch : AdvancedComponent, IPistolGun
{
    public void Shoot(Vector2 position, Vector2 direction, float rotationDegrees)
    {
        CM.GetComponent<ProjectileSpawner>()
            .Start()
            .SetPosition(position)
            .SetRotation(rotationDegrees)
            .SetDirectionNormal(direction)
            .Build();
        
        CM.GetComponent<ProjectileSpawner>().AddToGame();
    }
}