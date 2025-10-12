using System.Reflection.Metadata.Ecma335;
using Godot;
using TENamespace.health;
using TerraEngineer.entities.mobs;

namespace TerraEngineer.entities.projectiles;

public partial class Projectile : Mob
{
    [Export] private int damage = 1;
    [Export] private int piercing = 0;
    [Export] private bool breakOnWall = true;
    
    // Set by builder
    public Vector2 DirectionNormal;
    
    private void onBodyEntered(Node2D body)
    {
        if (body is Mob mob)
        {
            if (mob.Team != Team)
            {
                Health healthComp = mob.CM.TryGetComponent<Health>();
                healthComp?.ChangeHealth(-damage);
                pierceOrDie();
            }
            // Else ignore
        }
        else
        {
            // Hit solid ground
            if(breakOnWall) Die();
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
            Die();
        }
    }
}