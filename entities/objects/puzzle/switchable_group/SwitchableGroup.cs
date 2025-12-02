using System.Collections.Generic;
using System.Linq;
using Godot;

namespace TerraEngineer.entities.objects.puzzle.switchable_group;

[Tool]
public partial class SwitchableGroup : Node2D, ISwitchable
{
    [Export] private Color GroupColor
    {
        get => groupColor;
        set
        {
            groupColor = value;
            updateGroupColor();
        }
    }
    
    private Color groupColor;
    
    [Export] public StringName[] SavePropertiesNeededToSwitch { get; set; }
    [Export] public Node2D[] SwitchersNeededToSwitch { get; set; }
    [Export] public Node2D[] SwitchableGroupMembers;
    [Export] public bool GroupSwitchedOn { get; set; }

    public List<ISwitcher> Switchers { get; set; }
    public Node2D Me { get; set; }

    public override void _Ready()
    {
        #if TOOLS
        if (Engine.IsEditorHint())
            return;
        #endif
        
        ((ISwitchable)this).Init(this);
        
        updateGroupColor();
    }

    public void OnSwitch(bool switchedOn)
    {
        foreach (var switchable in SwitchableGroupMembers)
        {
            ((ISwitchableDependent)switchable).OnSwitch(switchedOn);
        }
    }
    
    private void updateGroupColor()
    {
        if(SwitchableGroupMembers == null) 
            return;
        
        foreach (var switchable in SwitchableGroupMembers)
        {
            AnimatedSprite2D sprite = switchable.GetNode<AnimatedSprite2D>(Names.Node.AnimatedSprite2D);
            sprite.Modulate = groupColor;
        }
    }
}