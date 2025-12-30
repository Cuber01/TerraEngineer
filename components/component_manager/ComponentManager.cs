using System;
using System.Collections.Generic;
using Godot;
using TerraEngineer.entities.mobs;

namespace TENamespace;

[Tool]
public partial class ComponentManager : Node2D
{
    private readonly Dictionary<Type, Component> components = new();
    private Entity actor = null;
    
    private bool lateInitialized = false;
    
    public override void _Ready()
    {
        #if TOOLS
        if (Engine.IsEditorHint())
            return;
        #endif
        
        if (GetParent<Node2D>() is Entity mob)
        {
            actor = mob;
        }
        
        Godot.Collections.Array<Node> children = GetChildren();
        foreach (Node child in children)
        {
            components.Add(child.GetType(), child as Component);
        }
        
        InitComponents();
    }
    
    public T TryGetComponent<T>() where T : Component
    {
        bool found = components.TryGetValue(typeof(T), out Component component);
        if (found)
        {
            return (T)component;
        }
        else
        {
            return null;    
        }
    }

    public T GetComponent<T>() where T : Component {
        return (T) components[typeof(T)];
    }

    public void InitComponents()
    {
        // Do not pass actor if we are a subcomponent
        if (actor != null)
        {
            foreach (var pair in components)
            {
                pair.Value.Init(actor);
            }    
        }
    }
    
    public void OptionalInitComponents()
    {
        if (actor != null)
        {
            foreach (var pair in components)
            {
                pair.Value.OptionalInit(actor);
            }    
        }
    }

    public void UpdateComponents(float delta)
    {
        foreach (var pair in components)
        {
            pair.Value.Update(delta);
        }
    }
}