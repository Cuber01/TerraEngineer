using Godot;

namespace TENamespace.basic.builders;

public abstract partial class Spawner : Component
{
    [Export] protected PackedScene Scene;
    
    protected Node Main;

    public override void _Ready()
    {
        Main = GetTree().GetRoot().GetNode("Main");
    }

    public void AddToGame(Node2D instance, Node2D parent = null)
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
    }

    protected Node2D SetTransform(Node2D instance, Vector2 position, float rotationDegrees)
    {
        instance.GlobalPosition = position;
        instance.RotationDegrees = rotationDegrees;
        return instance;
    }
}