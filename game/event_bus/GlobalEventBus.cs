using Godot;
using Godot.Collections;

namespace TerraEngineer.game;

// This Event Queue handles events of 'global' scope. So communication between
// unrelated high-level objects, but NOT between e.g. nodes and their children
public enum GlobalEvents
{
    BossDefeated
}

public partial class GlobalEventBus : Node2D
{
    public static EventBus<GlobalEvents> Instance;
}