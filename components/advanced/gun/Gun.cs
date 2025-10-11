using Godot;
using System;
using TENamespace.advanced;
using TENamespace.projectile_builder;
using TerraEngineer.entities.mobs;

public partial class Gun : AdvancedComponent
{
    [Export] private Node2D up;
    [Export] private Node2D down;
    [Export] private Node2D right;
    [Export] private Node2D left;
    
    public void Shoot(Directions4 dir)
    {
        Vector2 position = Vector2.Zero;
        Vector2 direction = Vector2.Zero;
        float rotationDegrees = 0;
        switch (dir)
        {
            case Directions4.Up:
                position = up.GlobalPosition;
                direction = Vector2.Up;
                rotationDegrees = 270;
                break;
            case Directions4.Down:
                position = down.GlobalPosition;
                direction = Vector2.Down;
                rotationDegrees = 90;
                break;
            case Directions4.Left:
                position = left.GlobalPosition;
                direction = Vector2.Left;
                rotationDegrees = 180;
                break;
            case Directions4.Right:
                position = right.GlobalPosition;
                direction = Vector2.Right;
                rotationDegrees = 0;
                break;
        }
        
        CM.GetComponent<ProjectileBuilder>().Build(position, direction, rotationDegrees);
    }
}
