using Godot;
using System;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.projectiles.gravity_bullet;

namespace TENamespace.basic.builders.gravity_bullet_spawner;

public partial class GravityBulletSpawner : Spawner<GravityBullet, GravityBulletSpawner>
{
    public GravityBulletSpawner SetVelocity(Vector2 velocity)
    {
        Instance.velocity = velocity;
        return this;
    }
    
    public GravityBulletSpawner SetTeam(CollisionTeam team)
    {
        Instance.Team = team;
        return this;
    }
    
    public GravityBulletSpawner SetOnCollideDeath(Action doOnCollideDeath)
    {
        Instance.OnCollideDeath = doOnCollideDeath;
        return this;
    }
    
    public GravityBulletSpawner SetOnLifetimeDeath(Action doOnLifetimeDeath)
    {
        Instance.OnLifetimeDeath = doOnLifetimeDeath;
        return this;
    }
}