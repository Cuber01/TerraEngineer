using Godot;
using System;
using System.Collections.Generic;
using TENamespace.advanced;
using TENamespace.advanced.terraform_gun;
using TENamespace.basic.particle_builder;
using TENamespace.projectile_builder;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.objects;

public interface IGun
{
    public void Shoot(Vector2 position, Vector2 direction, float rotationDegrees, bool mobParent);
}

public partial class GunHandle : AdvancedComponent
{
    [Export] private Node2D up;
    [Export] private Node2D down;
    [Export] private Node2D right;
    [Export] private Node2D left;
    [Export] private int selectedGun = 1;
    
    private List<IGun> guns = new List<IGun>();

    public override void _Ready()
    {
        guns.Add(CM.GetComponent<TerraformGun>());
    }
    
    public void Shoot(Directions4 dir, bool mobParent)
    {
        Vector2 position = Vector2.Zero;
        Vector2 direction = Vector2.Zero;
        float rotationDegrees = 0;
        switch (dir)
        {
            case Directions4.Up:
                position = mobParent ? up.Position : up.GlobalPosition;
                direction = Vector2.Up;
                rotationDegrees = 270;
                break;
            case Directions4.Down:
                position = mobParent ? down.Position : down.GlobalPosition;
                direction = Vector2.Down;
                rotationDegrees = 90;
                break;
            case Directions4.Left:
                position = mobParent ? left.Position : left.GlobalPosition;
                direction = Vector2.Left;
                rotationDegrees = 180;
                break;
            case Directions4.Right:
                position = mobParent ? right.Position : right.GlobalPosition;
                direction = Vector2.Right;
                rotationDegrees = 0;
                break;
        }
        
        guns[selectedGun].Shoot(position, direction, rotationDegrees, mobParent);

        //CM.GetComponent<ProjectileBuilder>().Build(position, direction, rotationDegrees, mobParent ? Actor : null);
    }
}
