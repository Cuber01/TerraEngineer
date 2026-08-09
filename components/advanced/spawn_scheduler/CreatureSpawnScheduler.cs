using Godot;
using TENamespace.basic.builders.creature_builder;

namespace TENamespace.advanced.spawn_scheduler;

public partial class CreatureSpawnScheduler : SpawnScheduler
{
    [Export] private CreatureSpawner spawner;
    
    protected override void Spawn(Vector2 position)
    {
        spawner
            .Start()
            .SetPosition(position)
            .Build();
        spawner.AddToGame();
    }
}