using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;
using TerraEngineer.entities.mobs;

namespace TENamespace;

public partial class ComponentManager : Node2D
{
    private readonly System.Collections.Generic.Dictionary<Type, Component> components = new System.Collections.Generic.Dictionary<Type, Component>();
    private Mob actor;
    
    public override void _Ready()
    {
        actor = GetParent<Mob>();
        
        Array<Node> children = GetChildren();
        foreach (Node child in children)
        {
            components.Add(child.GetType(), child as Component);
        }
        
        InitComponents();
    }

    public T GetComponent<T>() where T : Component {
        return (T) components[typeof(T)];
    }
    
    public void AddComponent(Component component) {}
    
    public void RemoveComponent(Component component) {}

    public void InitComponents()
    {
        foreach (var pair in components)
        {
            pair.Value.Init(actor);
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