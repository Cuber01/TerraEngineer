using Godot;

namespace TENamespace;

public partial class Jump : Component
{
    [Export] private float jumpVelocity = -100;
    [Export] private int maxJumps = 10;
    
    private int currentJumps = 0;

    public bool AttemptJump()
    {
        if (canJump())
        {
            executeJump();
            return true;
        }

        return false;
    }

    private void executeJump()
    {
        Actor.velocity.Y += jumpVelocity;
        currentJumps++;
    }

    private bool canJump()
    {
        return currentJumps < maxJumps;
    }
    
    
    // Signal: PLayer hit floor, reset current jumps
}