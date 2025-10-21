using Godot;
using TerraEngineer.entities.projectiles;

namespace TENamespace.projectile_builder;

public partial class ProjectileBuilder : Component
{
    [Export] private PackedScene projectileScene;

    private Node main;

    public override void _Ready()
    {
        main = GetTree().GetRoot().GetNode("Main");
    }

    public Projectile Build(Vector2 position, Vector2 directionNormal, float rotationDegrees, Node2D parent=null)
    {
        Projectile instance = (Projectile)projectileScene.Instantiate();
        instance.DirectionNormal = directionNormal;
        instance.GlobalPosition = position;
        instance.RotationDegrees = rotationDegrees;
        if (parent != null)
        {
            parent.AddChild(instance);
        }
        else
        {
            Node level = (Node2D)main.Get("CurrentLevel");
            level.AddChild(instance);    
        }
        
        return instance;
    }
}