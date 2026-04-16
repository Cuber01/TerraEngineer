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
    
    private void onBodyEntered(Node2D body)
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