using Godot;
using System;
using TENamespace.basic;
using TENamespace.health;
using TENamespace.lifetime;
using TerraEngineer.entities.mobs;

namespace TerraEngineer.entities.projectiles;

public partial class Projectile : Entity
{
    [Export] private Area2D hitArea;
    
    [Export] private int damage = 1;
    [Export] private int piercing = 0;
    [Export] private float knockbackForce = 100f;
    [Export] private bool breakOnWall = true;
    
    // Set by builder
    public Vector2 DirectionNormal;
    public Action OnLifetimeDeath;
    public Action OnCollideDeath;
    public Node2D Creator;
    public ulong CreatorId;

    public override void _Ready()
    {
        CM.InitComponents();
        if(OnLifetimeDeath != null)
            CM.GetComponent<Lifetime>().LifetimeEnded += () => OnLifetimeDeath.Invoke();
    }
    
    private void onAreaEntered(Area2D area)
    {
        OnDeflect();
    }
    
    private void onBodyEntered(Node2D body)
    {
        OnHit(body);
    }

    protected void OnHit(Node2D body)
    {
        if (body is Projectile antiProjectile)
        {
            pierceOrDie();
            antiProjectile.OnAntiProjectileHit(this);
        }
        else if (body is Entity mob)
        { 
            mob.CM?.TryGetComponent<Health>()
                ?.ChangeHealth(-damage, this);
                
            mob.CM?.TryGetComponent<KnockbackComponent>()
                ?.ApplyKnockback(GlobalPosition, knockbackForce);
                
            pierceOrDie();
        }
        else // Body is solid ground
        {
            if (breakOnWall)
            {
                OnCollideDeath?.Invoke();
                Die();
            }
        }
    }

    protected virtual void OnDeflect()
    {
        DirectionNormal = -DirectionNormal;
        velocity = -velocity;
        CM.GetComponent<FreeFly>().MultiplyAcceleration(2);
        ReverseCollision();
    }

    protected void ReverseCollision()
    {
        if (hitArea.GetCollisionMaskValue(Names.CollisionLayers.Player) && GetCollisionMaskValue(Names.CollisionLayers.Enemy))
        {
            // Do nothing (neutral bullet)
        } else if (hitArea.GetCollisionMaskValue(Names.CollisionLayers.Player)) 
        {
            // Enemy bullet -> Player bullet
            hitArea.SetCollisionMaskValue(Names.CollisionLayers.Enemy, true);
            hitArea.SetCollisionMaskValue(Names.CollisionLayers.Player, false);
        } else if (hitArea.GetCollisionMaskValue(Names.CollisionLayers.Enemy)) 
        {
            // Player bullet -> Enemy bullet
            hitArea.SetCollisionMaskValue(Names.CollisionLayers.Enemy, false);
            hitArea.SetCollisionMaskValue(Names.CollisionLayers.Player, true);
        }
    }

    public void OnAntiProjectileHit(Projectile victim) => pierceOrDie();   

    private void pierceOrDie()
    {
        if (piercing > 0)
        {
            piercing--;
        }
        else
        {
            OnCollideDeath?.Invoke();
            Die();
        }
    }
    
}