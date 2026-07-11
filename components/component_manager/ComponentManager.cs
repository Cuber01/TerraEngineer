using System;
using Godot;

namespace TENamespace;

[Tool]
public partial class ComponentManager : Node2D
{
    private readonly System.Collections.Generic.Dictionary<Type, Component> components = new();
    private Node2D actor = null;
    
    private bool lateInitialized = false;
    
    public override void _Ready()
    {
        #if TOOLS
        if (Engine.IsEditorHint())
            return;
        #endif
        
        if (GetParent() is Node2D mob)
        {
            actor = mob;
        }
        
        Godot.Collections.Array<Node> children = GetChildren();

        foreach (Node child in children) 
        {
            if (child is Component comp)
            {
                if (!components.TryAdd(child.GetType(), comp))
                {
                    #if DEBUG
                    GD.Print("Warning: Duplicate component key makes component " + child.GetType().Name + "inaccessible.");
                    #endif
                }
            }
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

    public bool HasComponent<T>() where T : Component
    {
        return components.TryGetValue(typeof(T), out Component _);
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
    
    public void AddComponent(Component component)
    {
        //try {
            components.Add(component.GetType(), component);
        //catch
            component.Init(actor);
    }
    

    public void RemoveComponent<T>() where T : Component
    {
        components[typeof(T)].OnRemoved();
        components.Remove(typeof(T));
    }
}