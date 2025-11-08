using Godot;
using TerraEngineer;
using TerraEngineer.entities.mobs;

namespace TENamespace;

public partial class Jump : Component
{
    [Export] private float jumpVelocity = 100;
    [Export] public int MaxJumps = 2;
    [Export] private float limitThreshold = 30f;
    
    private int currentJumps = 0;

    public override void Init(Entity actor)
    {
        base.Init(actor);
        actor.CM.GetComponent<Gravity>().LandedOnFloor += 
            () => currentJumps = 0;
    }
    
    public bool AttemptJump(float forceMultiplier=1f)
    {
        if (canJump())
        {
            executeJump(forceMultiplier);
            return true;
        }

        return false;
    }

    public void CancelJump()
    {
        Actor.velocity.Y = Mathf.Clamp(Actor.velocity.Y, 0, MathT.POSITIVE_INF);
    }

    public void LimitJump()
    {
        if (Actor.velocity.Y <= -limitThreshold)
        {
            Actor.velocity.Y += limitThreshold;
        }
    }

    private void executeJump(float forceMultiplier=1f)
    {
        Actor.velocity.Y = -jumpVelocity*forceMultiplier;
        currentJumps++;
    }

    private bool canJump()
    {
        return currentJumps < MaxJumps;
    }
    
}