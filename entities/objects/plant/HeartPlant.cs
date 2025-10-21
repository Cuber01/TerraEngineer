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

    private ITimer regrowTimer;
    
    private bool hasFruit = true;

    public override void Enable()
    {
        base.Enable();
        if (!hasFruit)
        {
            regrowTimer = TimerManager.Schedule(secondsTilRegrow, (_) => regrow());
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
            regrowTimer = TimerManager.Schedule(secondsTilRegrow, (_) => regrow());
        }
    }

    private void regrow()
    {
        if(!regrowAllowed) return;

        hasFruit = true;
        sprite.Animation = "default";    
    }
    
    public override void Disable()
    {
        base.Disable();
        TimerManager.Cancel(regrowTimer);
    }
    
    public override void _ExitTree() =>
        TimerManager.Cancel(regrowTimer);
    
}
