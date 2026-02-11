using System;
using System.Collections.Generic;
using Godot;
using TerraEngineer.entities.mobs;

namespace TerraEngineer.game;

public class Controller
{
    public bool TurnActive = false;
    private bool active = false;
    private Controller switchedFrom = null;
    
    private Dictionary<StringName, Action> actions = new();
    private Dictionary<StringName, Action> overrides = new();
    
    private Dictionary<StringName, Action> releaseActions = new();
    
    // Actions in an exclusive group will be executed only once per update
    private Dictionary<StringName, StringName> exclusiveGroups = new();

    public void Update(float delta)
    {
        if (TurnActive)
        {
            active = true;
            TurnActive = false;
            return;
        } else if (!active) {
            return;
        }
        
        
        HashSet<StringName> executedGroups = new();
        
        foreach (KeyValuePair<StringName, Action> pair in actions)
        {
            if (Input.IsActionJustPressed(pair.Key))
            {
                // Input overriden, eg. shoot is overriden by levers
                if (overrides.ContainsKey(pair.Key))
                {
                    overrides[pair.Key]();
                }
                else
                {
                    if (exclusiveGroups.ContainsKey(pair.Key))
                    {
                        if (!executedGroups.Contains(pair.Key))
                        {
                            executedGroups.Add(pair.Key);
                            pair.Value();
                        }
                    }
                    else
                    {
                        pair.Value();
                    }    
                }
            }
        }

        foreach (KeyValuePair<StringName, Action> pair in releaseActions)
        {
            if (Input.IsActionJustReleased(pair.Key))
            {
                pair.Value();
            }
        }
    }

    public DirectionX GetAxis(StringName negativeAction, StringName positiveAction)
    {
        return (DirectionX)(int)Input.GetAxis(negativeAction, positiveAction);
    }
    
    public void AddAction(StringName actionName, Action action, StringName exclusiveGroup = null)
    {
        actions[actionName] = action;
        
        if(exclusiveGroup != null)
            exclusiveGroups[actionName] = exclusiveGroup;
    }

    public void AddReleaseAction(StringName actionName, Action action)
    {
        releaseActions[actionName] = action;
    }
    
    public void AddOverride(StringName actionName, Action? action)
    {
        overrides[actionName] = action;
    }
    
    public void RemoveOverride(StringName actionName)
    {
        overrides.Remove(actionName);
    }

    public void SwitchControl(Controller to)
    {
        active = false;
        to.TurnActive = true;
        to.switchedFrom = this;
    }

    public void GiveBackControl()
    {
        switchedFrom.TurnActive = true;
        switchedFrom.switchedFrom = this;
        switchedFrom = null;
        active = false;
    }

}