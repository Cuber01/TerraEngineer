using Godot;
using System;
using TENamespace.basic;
using TENamespace.health;
using TENamespace.lifetime;
using TerraEngineer.entities.mobs;

namespace TerraEngineer.entities.projectiles;

public partial class Projectile : Entity
{
    [Export] private int damage = 1;
    [Export] private int piercing = 0;
    [Export] private float knockbackForce = 100f;
    [Export] private bool breakOnWall = true;
    
    // Set by builder
    public Vector2 DirectionNormal;
    public Action OnLifetimeDeath;
    public Action OnCollideDeath;

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
        if (body is Entity mob)
        {
            if (mob.Team != Team)
            {
                Health healthComp = mob.CM.TryGetComponent<Health>();
                healthComp?.ChangeHealth(-damage);
                
                mob.CM.TryGetComponent<KnockbackComponent>()
                    ?.ApplyKnockback(GlobalPosition, knockbackForce);
                
                pierceOrDie();
            }
            // Else ignore
        }
        else
        {
            // Hit solid ground

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
        CM.GetComponent<FreeFly>().MultiplyAcceleration(2);
        ReverseTeams();
    }

    protected void ReverseTeams()
    {
        if (GetCollisionMaskValue(Names.CollisionLayers.Player) && GetCollisionMaskValue(Names.CollisionLayers.Enemy))
        {
            // Do nothing (neutral bullet)
        } else if (GetCollisionMaskValue(Names.CollisionLayers.Player)) 
        {
            // Enemy bullet -> Player bullet
            Team = CollisionTeam.Player;
            SetCollisionMaskValue(Names.CollisionLayers.Enemy, true);
            SetCollisionMaskValue(Names.CollisionLayers.Player, false);
        } else if (GetCollisionMaskValue(Names.CollisionLayers.Enemy)) 
        {
            // Player bullet -> Enemy bullet
            Team = CollisionTeam.Enemy;
            SetCollisionMaskValue(Names.CollisionLayers.Enemy, false);
            SetCollisionMaskValue(Names.CollisionLayers.Player, true);
        }
    }

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