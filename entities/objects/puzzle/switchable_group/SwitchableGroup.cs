using System.Collections.Generic;
using Godot;

namespace TerraEngineer.entities.objects.puzzle.switchable_group;

public partial class SwitchableGroup : Node2D, ISwitchable
{
    [Export] private Color groupColor;
    
    [Export] public StringName[] SavePropertiesNeededToSwitch { get; set; }
    [Export] public Node2D[] SwitchersNeededToSwitch { get; set; }
    [Export] public Node2D[] SwitchableGroupMembers;
    [Export] public bool GroupSwitchedOn { get; set; }
    
    public List<ISwitcher> Switchers { get; set; }
    public Node2D Me { get; set; }

    public override void _Ready()
    {
        ((ISwitchable)this).Init(this);
    }
    
    public void OnSwitch(bool switchedOn)
    {
        foreach (var switchable in SwitchableGroupMembers)
        {
            ((ISwitchableDependent)switchable).OnSwitch(switchedOn);
        }
    }
}