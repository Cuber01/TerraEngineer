using Godot;
using TENamespace.advanced.main_gun_wrapper;
using TENamespace.projectile_builder;

namespace TENamespace.advanced.anti_projectile_launcher;

public partial class AntiProjectileLauncher : AdvancedComponent, IPistolGun
{
    public void Shoot(Vector2 position, Vector2 direction, float rotationDegrees)
    {
        CM.GetComponent<ProjectileSpawner>()
            .Start()
            .SetPosition(position)
            .SetRotation(rotationDegrees)
            .SetCreator(Actor)
            .Build();
        
        CM.GetComponent<ProjectileSpawner>().AddToGame();
    }
}