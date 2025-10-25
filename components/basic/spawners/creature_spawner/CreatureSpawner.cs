using Godot;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.projectiles;

namespace TENamespace.basic.builders.creature_builder;

public partial class CreatureSpawner : Spawner<Mob, CreatureSpawner>
{
    public void SetFacing(DirectionX facing)
    {
        Instance.Flip(facing);
    }
}