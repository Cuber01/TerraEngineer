using System;
using Godot;
using TENamespace.basic.particle_builder;
using TerraEngineer;

namespace TENamespace.basic.builders;

public partial class Spawner<T, Me> : Component where T : Node2D where Me: Spawner<T, Me>
{
    [Export] protected PackedScene Scene;

    protected Node Main;
    protected T Instance;

    public override void _Ready()
    {
        Main = GetTree().GetRoot().GetNode(Names.Node.Main);
    }

    public virtual T Build()
    {
        return Instance;
    }

    public Me AddToGame(Node2D parent = null)
    {
        if (parent != null)
        {
            parent.AddChild(Instance);
        }
        else
        {
            Node level = (Node2D)Main.Get(Names.Properties.CurrentLevel);
            level.AddChild(Instance);    
        }

        return (Me)this;
    }
    
    public Me Start(PackedScene overrideScene=null)
    {
        if (overrideScene != null)
        {
            Scene = overrideScene;
        }
        
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