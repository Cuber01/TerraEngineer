using Godot;
using Godot.Collections;

namespace TerraEngineer.game;

// This Event Queue handles events of 'global' scope. So communication between
// unrelated high-level objects, but NOT between e.g. nodes and their children
public enum GlobalEvents
{
    BossEntered,
    BossDefeated,
    None
}

public partial class GlobalEventBus : Node2D
{
    public static EventBus<GlobalEvents> Instance;

    public override void _Ready()
    {
        Instance = new EventBus<GlobalEvents>();
    }
}