using Godot;
using TENamespace.advanced.main_gun_wrapper;
using TENamespace.projectile_builder;
using Vector2 = System.Numerics.Vector2;

namespace TENamespace.advanced.shotgun;

public partial class Shotgun : AdvancedComponent, IMainGun
{
    public void Shoot(Godot.Vector2 position, Godot.Vector2 direction, float rotationDegrees)
    {
        CM.GetComponent<ProjectileSpawner>()
            .Start()
            .SetPosition(position)
            .SetDirectionNormal(direction)
            .Build();

        CM.GetComponent<ProjectileSpawner>().AddToGame();
    }
}