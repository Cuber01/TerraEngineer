using System;
using System.Collections.Generic;
using Godot;

namespace TerraEngineer.game;

public class Controller
{
    private Dictionary<StringName, Action> actions = new();
    private Dictionary<StringName, Action> overrides = new();
    
    // Actions in an exclusive group will be executed only once per update
    private Dictionary<StringName, StringName> exclusiveGroups = new();

    public void Update(float delta)
    {
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
    }
    
    public void AddAction(StringName actionName, Action action, StringName exclusiveGroup = null)
    {
        actions[actionName] = action;
        
        if(exclusiveGroup != null)
            exclusiveGroups[actionName] = exclusiveGroup;
    }
    
    public void AddOverride(StringName actionName, Action action)
    {
        overrides[actionName] = action;
    }
    
    public void RemoveOverride(StringName actionName)
    {
        overrides.Remove(actionName);
    }
    

}