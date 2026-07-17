using Godot;
using System;

public partial class GlobalDialoguesState : Node
{
    // 1. A static pointer to this specific instance
    public static GlobalDialoguesState Instance { get; private set; }

    // 2. The variable itself MUST be a public, non-static property
    [Export]
    public int PuzzleTerraformingRoom_Biome { get; set; } = 0;
    
    [Export]
    public int PuzzleFridge_Choice { get; set; } = 0;
    
    [Export]
    public int Lab_InventoryTaker { get; set; } = 0;
    
    [Export]
    public int PuzzlePlant_Choice { get; set; } = 0;

    public override void _EnterTree()
    {
        // 3. Assign the static instance pointer when the node loads
        Instance = this;
    }
}
