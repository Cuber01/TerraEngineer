using Godot;
using TerraEngineer.entities.mobs;

namespace TENamespace;

public partial class Dash : Component
{
    [Export] private float dashSpeed = 400;
    [Export] private float dashDuration = 0.06f;
    [Export] private int maxDashes = 1;

    private int currentDashes = 0;
    private bool isDashing = false;
    private int dashDirection;
    
    public override void Init(Mob actor)
    {
        base.Init(actor);
        actor.CM.GetComponent<Gravity>().LandedOnFloor += 
            () => currentDashes = 0;
    }
    
    public override void Update(float delta)
    {
        if (isDashing)
        {
            Actor.velocity.X = dashSpeed * dashDirection;
            Actor.velocity.Y = 0;
        }
    }

    public bool AttemptDash(DirectionX direction)
    {
        if (canDash())
        {
            executeDash(direction);  
            return true;
        }
        return false;
    }

    private void executeDash(DirectionX direction)
    {
        isDashing = true;
        dashDirection = (int)direction;
        TimerManager.Schedule(dashDuration, endDash);
        currentDashes++;
    }

    private void endDash(ITimer timer)
    {
        isDashing = false;
    }

    private bool canDash()
    {
        return currentDashes < maxDashes;
    }
    
}