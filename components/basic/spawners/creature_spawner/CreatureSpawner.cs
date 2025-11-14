using Godot;
using TENamespace.health;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.projectiles;

namespace TENamespace.basic.builders.creature_builder;

[Tool]
public partial class CreatureSpawner : Spawner<Entity, CreatureSpawner>
{
    public CreatureSpawner SetHealth(int health)
    {
        Instance.CM.GetComponent<Health>().SetHealth(health);
        return this;
    }
    
    public CreatureSpawner SetFacing(DirectionX facing)
    {
        Instance.Flip(facing);
        return this;
    }
}