using Godot;
using System.Collections.Generic;
using System.Linq;

namespace TerraEngineer.entities.objects.puzzle.switchable_group;

public partial class SwitcherGroup : Node2D
{
    [Export] public StringName[] SavePropertiesToSwitch { get; set; }
    public List<ISwitcher> SwitchersToSwitch = new List<ISwitcher>();

    // False = blocks impassable, true = blocks passable
    public bool GroupSwitchedOn = false;
    public bool IsInit = false;
    private SwitchableGroup mySwitchableGroup;
    
    public void Init(SwitchableGroup switchableGroup)
    {
        foreach (var switcher in GetChildren())
        {
            SwitchersToSwitch.Add((ISwitcher)switcher);
        }
        
        foreach (var switcher in (IEnumerable<ISwitcher>)SwitchersToSwitch)
        { 
            switcher.Switched += checkSwitchers;
        }    
        
        if (SavePropertiesToSwitch != null)
        {
            SaveData.RealtimeDataChanged += checkSwitchers;    
        }
        
        IsInit = true;
    }
    
    private void checkSwitchers(bool switchedOn)
    {
        if(GroupSwitchedOn == switchedOn)
            return;

        if (switchedOn == false)
        {
            GroupSwitchedOn = false;
            mySwitchableGroup.OnSwitch(GroupSwitchedOn);
            return;
        }
        
        foreach (ISwitcher switcher in (IEnumerable<ISwitcher>)SwitchersToSwitch ?? Enumerable.Empty<ISwitcher>())
        {
            if (!switcher.SwitchedOn)
            {
                #if DEBUG
                GD.Print("Failed to switch: " + switcher + " was false.");
                #endif
                return;
            }
        }
        
        StringName levelName = (StringName)GetParent().GetMeta(Names.Properties.LevelName);
        
        foreach (string property in SavePropertiesToSwitch ?? Enumerable.Empty<StringName>())
        {
            Variant value = SaveData.ReadValue(levelName, property);
            if (value.VariantType == Variant.Type.Nil || (bool)value == false)
            {
                #if DEBUG
                GD.Print("Failed to switch: " + property + " was false.");
                #endif
                return;
            }
        }
        
        GroupSwitchedOn = !GroupSwitchedOn;
        mySwitchableGroup.OnSwitch(GroupSwitchedOn);
    }
}