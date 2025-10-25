using System;
using Godot;
using TENamespace.basic.particle_builder;

namespace TENamespace.basic.builders;

public partial class Spawner<T, Me> : Component where T : Node2D where Me: Spawner<T, Me>
{
    [Export] protected PackedScene Scene;

    protected Node Main;
    protected T Instance;

    public override void _Ready()
    {
        Main = GetTree().GetRoot().GetNode("Main");
    }

    public virtual T Build()
    {
        return Instance;
    }

    public void AddToGame(T instance, Node2D parent = null)
    {
        if (parent != null)
        {
            parent.AddChild(instance);
        }
        else
        {
            Node level = (Node2D)Main.Get("CurrentLevel");
            level.AddChild(instance);    
        }

        Instance = default(T);
    }
    
    public Me Start()
    {
        Instance = (T)Scene.Instantiate();
        return (Me)this;
    }
    
    public new virtual Me SetPosition(Vector2 position)
    {
        Instance.GlobalPosition = position;
        return (Me)this;
    }

    public new virtual Me SetRotation(float rotationDeg)
    {
        Instance.RotationDegrees = rotationDeg;
        return (Me)this;
    }


}