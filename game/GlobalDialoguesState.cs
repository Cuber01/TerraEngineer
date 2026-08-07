using Godot;
using System;

public partial class GlobalDialoguesState : Node
{
    public static GlobalDialoguesState Instance { get; private set; }

    [Export]
    public int PuzzleTerraformingRoom_Biome { get; set; } = 0;
    
    [Export]
    public int PuzzleFridge_Choice { get; set; } = 0;
    
    [Export]
    public int Lab_CraftingStation { get; set; } = 0;
    
    [Export]
    public int Lab_InventoryTaker { get; set; } = 0;
    
    [Export]
    public int PuzzlePlant_Choice { get; set; } = 0;

    public override void _EnterTree()
    {
        Instance = this;
    }
}
