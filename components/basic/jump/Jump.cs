using Godot;
using TerraEngineer.entities.mobs;

namespace TENamespace;

public partial class Jump : Component
{
    [Export] private float jumpVelocity = 100;
    [Export] public int MaxJumps = 2;
    
    private int currentJumps = 0;

    public override void Init(Mob actor)
    {
        base.Init(actor);
        actor.CM.GetComponent<Gravity>().LandedOnFloor += 
            () => currentJumps = 0;
    }
    
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
        Actor.velocity.Y = -jumpVelocity;
        currentJumps++;
    }

    private bool canJump()
    {
        return currentJumps < MaxJumps;
    }
    
}