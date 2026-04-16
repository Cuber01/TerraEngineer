using Godot;
using System;
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

    public ProjectileSpawner SetOnCollideDeath(Action doOnCollideDeath)
    {
        Instance.OnCollideDeath = doOnCollideDeath;
        return this;
    }
    
    public ProjectileSpawner SetOnLifetimeDeath(Action doOnLifetimeDeath)
    {
        Instance.OnCollideDeath = doOnLifetimeDeath;
        return this;
    }
}