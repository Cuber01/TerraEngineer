using System.Collections.Generic;
using Godot;

namespace TerraEngineer.entities.objects.puzzle.switchable_group;

public class SwitchableGroup : ISwitchable
{
    [Export] private Color groupColor;
    
    [Export] public StringName[] SavePropertiesNeededToSwitch { get; set; }
    [Export] public Node2D[] SwitchersNeededToSwitch { get; set; }
    [Export] public Node2D[] SwitchableGroupMembers;
    
    public List<ISwitcher> Switchers { get; set; }
    public Node2D Me { get; set; }

    public void OnSwitch()
    {
        foreach (var switchable in SwitchableGroupMembers)
        {
            ((ISwitchableDependent)switchable).OnSwitch();
        }
    }
}