using Godot;
using System;
using TerraEngineer.entities;

public partial class Mushroom : Terraformable
{
    [Export] private float bounceVelocity = 150;
    
    private void onBodyEntered(Node2D body)
    {
        Player player = (Player)body;
        if (player.velocity.Y > 0)
        {
            player.velocity.Y = -bounceVelocity;
        }
    }
}
