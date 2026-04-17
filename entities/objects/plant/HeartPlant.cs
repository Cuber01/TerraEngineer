using Godot;
using TENamespace.health;
using TerraEngineer;
using TerraEngineer.entities;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.objects;

public partial class HeartPlant : Terraformable
{
    [Export] private AnimatedSprite2D sprite;
    private const int HealthHealed = 3;
    private const float SecondsTilRegrow = 5;
    private bool regrowAllowed = true;
    
    private ITimer regrowTimer;
    
    private bool hasFruit = true;
    
    public override void Enable()
    {
        base.Enable();
        if (!hasFruit)
        {
            regrowTimer = TimerManager.Schedule(SecondsTilRegrow, this, (_) => regrow());
        }
    }

    private void onBodyEntered(Node2D body)
    {
        if (hasFruit)
        {
            Player player = (Player)body;
            sprite.Animation = Names.Animations.NoFruit;
            player.CM.GetComponent<Health>().ChangeHealth(HealthHealed);
            hasFruit = false;    
            regrowTimer = TimerManager.Schedule(SecondsTilRegrow, this, (_) => regrow());
        }
    }

    private void regrow()
    {
        if(!regrowAllowed) return;

        hasFruit = true;
        sprite.Animation = Names.Animations.Default;    
    }
    
    public override void Disable()
    {
        base.Disable();
        TimerManager.Cancel(regrowTimer);
    }
    
}
