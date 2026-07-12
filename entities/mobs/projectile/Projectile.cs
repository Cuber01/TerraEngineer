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
                healthComp?.ChangeHealth(-damage, this);
                
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
        velocity = -velocity;
        CM.GetComponent<FreeFly>().MultiplyAcceleration(2);
        ReverseTeams();
    }

    protected void ReverseTeams()
    {
        if (hitArea.GetCollisionMaskValue(Names.CollisionLayers.Player) && GetCollisionMaskValue(Names.CollisionLayers.Enemy))
        {
            // Do nothing (neutral bullet)
        } else if (hitArea.GetCollisionMaskValue(Names.CollisionLayers.Player)) 
        {
            // Enemy bullet -> Player bullet
            Team = CollisionTeam.Player;
            hitArea.SetCollisionMaskValue(Names.CollisionLayers.Enemy, true);
            hitArea.SetCollisionMaskValue(Names.CollisionLayers.Player, false);
        } else if (hitArea.GetCollisionMaskValue(Names.CollisionLayers.Enemy)) 
        {
            // Player bullet -> Enemy bullet
            Team = CollisionTeam.Enemy;
            hitArea.SetCollisionMaskValue(Names.CollisionLayers.Enemy, false);
            hitArea.SetCollisionMaskValue(Names.CollisionLayers.Player, true);
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