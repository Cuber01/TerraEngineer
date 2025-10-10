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

    public Projectile Build(Vector2 DirectionNormal)
    {
        Projectile instance = (Projectile)projectileScene.Instantiate();
        instance.DirectionNormal = DirectionNormal;
        root.AddChild(instance);
        return instance;
    }
}