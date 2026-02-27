using Godot;

namespace TerraEngineer.entities.objects.puzzle.switchable_group;

public partial class SwitcherGroup : Node2D
{
    [Export] public StringName[] SavePropertiesNeededToSwitch { get; set; }
    [Export] public Node2D[] SwitchersNeededToSwitch { get; set; }
    
    
}