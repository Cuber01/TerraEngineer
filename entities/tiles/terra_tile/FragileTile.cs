using Godot;
using TENamespace.basic.save_tile;
using TerraEngineer.entities;
using TerraEngineer.entities.tiles;

public partial class FragileTile : TerraformableEntity
{
    [Export] private Area2D stepDetector;

    private float shatterTime = 1.5f;
    private float repairTime = 3f;
    private ITimer currentTimer;
    
    public override void _Ready()
    {
        stepDetector.BodyEntered += steppedOn;
        //stepDetector.BodyEntered += steppedOff;
    }

    private void steppedOn(Node2D stepper)
    {
        currentTimer = TimerManager.Schedule(shatterTime, this, shatter);
    }

    // private void steppedOff(Node2D stepper)
    // {
    //     
    // }

    private void shatter(ITimer _)
    {
        if(!Active) return;
        
        Hide();
        stepDetector.SetDeferred(Area2D.PropertyName.Monitoring, false);
        Hitbox.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
        currentTimer = TimerManager.Schedule(repairTime, this, repair);
    }

    private void repair(ITimer _)
    {
        if(!Active) return;
        
        Show();
        stepDetector.SetDeferred(Area2D.PropertyName.Monitoring, true);
        Hitbox.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
    }

    public override void Enable()
    {
        stepDetector.SetDeferred(Area2D.PropertyName.Monitoring, true);
        base.Enable();
    }
    
    public override void Disable()
    {
        stepDetector.SetDeferred(Area2D.PropertyName.Monitoring, false);
        base.Disable();
    }

}
