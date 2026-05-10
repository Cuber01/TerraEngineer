using TENamespace.advanced.main_gun_wrapper;
using TENamespace.projectile_builder;

namespace TENamespace.advanced.shotgun;

public partial class Rifle : AdvancedComponent, IPistolGun
{
    private const float ReloadTime = 0.5f;
    private bool canShoot = true;
    private ITimer reloadTimer = null;
    
    public void Shoot(Godot.Vector2 position, Godot.Vector2 direction, float rotationDegrees)
    {
        if(!canShoot)
            return;
        
        reloadTimer = TimerManager.Schedule(ReloadTime, false, Actor, 
            (_) => canShoot = true);
        canShoot = false;
        
        CM.GetComponent<ProjectileSpawner>()
            .Start()
            .SetPosition(position)
            .SetRotation(rotationDegrees)
            .SetDirectionNormal(direction)
            .Build();

        CM.GetComponent<ProjectileSpawner>().AddToGame();
    }
}