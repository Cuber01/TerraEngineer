using Godot;
using System;
using TENamespace.health;
using TerraEngineer.entities;

public partial class HeartPlant : Terraformable
{
    [Export] private int healthHealed = 3;
    [Export] private float secondsTilRegrow = 5;
    [Export] private AnimatedSprite2D sprite;
    [Export] private bool regrowAllowed = true;

    private bool hasFruit = true;
    private bool timerScheduled = false;

    public override void Enable()
    {
        base.Enable();
        if (!timerScheduled && !hasFruit)
        {
            TimerManager.Schedule(secondsTilRegrow, (_) => regrow());
        }
    }

    private void onBodyEntered(Node2D body)
    {
        if (hasFruit)
        {
            Player player = (Player)body;
            sprite.Animation = "no_fruit";
            player.CM.GetComponent<Health>().ChangeHealth(healthHealed);
            hasFruit = false;    
            TimerManager.Schedule(secondsTilRegrow, (_) => regrow());
            timerScheduled = true;
        }
    }

    private void regrow()
    {
        if(!regrowAllowed) return;
        
        if (Active)
        {
            hasFruit = true;
            sprite.Animation = "default";    
        }

        timerScheduled = false;
    }
}
