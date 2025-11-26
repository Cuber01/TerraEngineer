using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace TerraEngineer.entities.objects.puzzle;

// ISwitchable can be switched by ISwitchers and SaveData level properties
public interface ISwitchable
{
    // If all of these properties exist and are set to true...
    public StringName[] SavePropertiesNeededToSwitch { get; set; }
    // ...and all the switches are true
    public Node2D[] SwitchersNeededToSwitch { get; set; }
    // ...then the switch action happens
    
    public List<ISwitcher> Switchers { get; set; }
    protected Node2D Me { get; set; }

    public void Init(Node2D me)
    {
        Me = me;
        Switchers = new List<ISwitcher>();
        
        foreach (var switcher in SwitchersNeededToSwitch)
        {
            Switchers.Add((ISwitcher)switcher);
        }
        
        foreach (var switcher in Switchers)
        {
            switcher.Switched += checkSwitchers;
        }
        
        SaveData.RealtimeDataChanged += checkSwitchers;
    }

    private void checkSwitchers(bool switchedOn)
    {
        if(!switchedOn) return;

        foreach (ISwitcher switcher in Switchers)
        {
            if (!switcher.SwitchedOn)
            {
                GD.Print("Failed to switch: " + switcher + " was false.");
                return;
            }
        }
        
        StringName levelName = (StringName)Me.GetParent().GetMeta(Names.Properties.LevelName);
        
        foreach (string property in SavePropertiesNeededToSwitch)
        {
            Variant? value = SaveData.ReadValue(levelName, property);
            if (value == null || (bool)value == false)
            {
                GD.Print("Failed to switch: " + property + " was false.");
                return;
            }
        }
        
        OnSwitch();
    }
    
    protected void OnSwitch();
}