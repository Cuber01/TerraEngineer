using Godot;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.projectiles;

namespace TENamespace.basic.builders.creature_builder;

public partial class CreatureSpawner : Spawner
{
    public Mob Build(Vector2 position, DirectionX facing)
    {
        Mob instance = (Mob)Scene.Instantiate();
        instance = (Mob)SetTransform(instance, position, 0);
        instance.Facing = facing;
        
        return instance;
    }
}