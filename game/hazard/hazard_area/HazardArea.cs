using Godot;
using System;
using TENamespace.health;

public partial class HazardArea : Area2D
{
    private const int Damage = 2;
    
    public override void _Ready()
    {
        BodyEntered += playerEntered;
    }

    private void playerEntered(Node2D player)
    {
        Player p = player as Player;
        p!.CM.GetComponent<Health>().ChangeHealth(-Damage);
        p!.ReturnToHazardRespawnPoint();
    }
    
}
