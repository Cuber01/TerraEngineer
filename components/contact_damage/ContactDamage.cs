using Godot;
using TENamespace.health;
using TerraEngineer.entities.mobs;

namespace TENamespace.contact_damage;

public partial class ContactDamage : Component
{
    [Export] private int contactDamageAmount = 1;
    
    private void onAttackAreaEnemyEntered(Node2D body)
    {
        if (body is Mob mob)
        {
            mob.CM.GetComponent<Health>().ChangeHealth(-contactDamageAmount);    
        }
    }
}