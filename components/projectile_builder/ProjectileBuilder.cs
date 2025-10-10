using Godot;
using TerraEngineer.entities.projectiles;

namespace TENamespace.projectile_builder;

public partial class ProjectileBuilder : Component
{
    [Export] private PackedScene projectileScene;

    private Node root;

    public override void _Ready()
    {
        root = GetTree().GetRoot().GetChild(0);
    }

    public Projectile Build(Vector2 position, Vector2 directionNormal)
    {
        Projectile instance = (Projectile)projectileScene.Instantiate();
        instance.DirectionNormal = directionNormal;
        instance.GlobalPosition = position;
        root.AddChild(instance);
        return instance;
    }
}