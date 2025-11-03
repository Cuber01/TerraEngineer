using Godot;
using TENamespace.basic;
using TENamespace.health;
using TerraEngineer.entities.mobs;

namespace TENamespace.contact_damage;

public partial class ContactDamage : Component
{
    [Export] private int contactDamageAmount = 1;
    [Export] private float knockbackForce = 500f;
    
    private void onAttackAreaEnemyEntered(Node2D body)
    {
        if (body is Mob mob)
        {
            mob.CM.TryGetComponent<Health>()?.ChangeHealth(-contactDamageAmount);
                
            mob.CM.TryGetComponent<KnockbackComponent>()
                ?.ApplyKnockback(GlobalPosition, knockbackForce);

        }
    }
}