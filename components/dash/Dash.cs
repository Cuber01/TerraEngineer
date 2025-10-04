using Godot;

namespace TENamespace;

public partial class Dash : Component
{
    [Export] private float dashSpeed = 100;
    [Export] private float dashDuration = 5f;
    [Export] private int maxDashes = 10;

    private int currentDashes = 0;
    private bool isDashing = false;
    private int dashDirection;
    
    public override void Update(float delta)
    {
        if (isDashing)
        {
            Actor.velocity.X = dashSpeed * dashDirection;
            Actor.velocity.Y = 0;
        }
    }

    public bool AttemptDash(int direction)
    {
        if (canDash())
        {
            executeDash(direction);  
            return true;
        }
        return false;
    }

    private void executeDash(int direction)
    {
        isDashing = true;
        dashDirection = direction;
        TimerManager.Schedule(dashDuration, endDash);
        currentDashes++;
    }

    private void endDash(ITimer timer)
    {
        isDashing = false;
        currentDashes = 0;
    }

    private bool canDash()
    {
        return currentDashes < maxDashes;
    }
    
    // Signal: PLayer hit floor, reset current dashes
}