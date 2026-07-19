using System;
using System.Collections.Generic;
using Godot;
using TerraEngineer.entities.mobs;

namespace TerraEngineer.game;

public class InputContext
{
    private Dictionary<StringName, Action> actions = new();
    
    // TODO No more overrides needed - just push other context
    // private Dictionary<StringName, Action> overrides = new();
    
    private Dictionary<StringName, Action> releaseActions = new();
    
    // Actions in an exclusive group will be executed only once per update
    private Dictionary<StringName, StringName> exclusiveGroups = new();

    public bool HandleNewInput(InputEvent @event)
    {
        HashSet<StringName> executedGroups = new();
        
        foreach (KeyValuePair<StringName, Action> pair in actions)
        {
            if (@event.IsActionPressed(pair.Key))
            {
                if (exclusiveGroups.ContainsKey(pair.Key))
                {
                    if (!executedGroups.Contains(pair.Key))
                    {
                        executedGroups.Add(pair.Key);
                        pair.Value();
                        return true;
                    }
                }
                else
                {
                    pair.Value();
                    return true;
                }    
            }
        }

        foreach (KeyValuePair<StringName, Action> pair in releaseActions)
        {
            if (@event.IsActionReleased(pair.Key))
            {
                pair.Value();
                return true;
            }
        }

        return false;
    }

    public void HandleEchoInput(InputEvent @event)
    {
        // Nothing? For now.
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


}