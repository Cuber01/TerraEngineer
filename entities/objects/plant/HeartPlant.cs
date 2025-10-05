using Godot;
using System;
using TENamespace.health;
using TerraEngineer.entities;

public partial class HeartPlant : Terraformable
{
    [Export] private int healthHealed = 3;
    [Export] private AnimatedSprite2D sprite;
    
    private void onBodyEntered(Node2D body)
    {
        Player player = (Player)body;
        sprite.Animation = "no_fruit";
        player.CM.GetComponent<Health>().ChangeHealth(healthHealed);
    }
}
