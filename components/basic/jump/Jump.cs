using Godot;
using System;
using System.Runtime.CompilerServices;
using TerraEngineer;
using TerraEngineer.entities.mobs;

namespace TENamespace;

public partial class Jump : Component
{
    [Export] private float jumpVelocity = 100;
    [Export] public int MaxJumps = 2;
    
    private const float LimitThreshold = 30f;
    
    
    public int CurrentJumps = 0;
    private Entity entityActor;

    public override void Init(Node2D actor)
    {
        base.Init(actor);
        if (actor is Entity entity)
        {
            entityActor = entity;
        }
        else
        {
            throw new Exception("Jump component requires Entity actor.");
        }
        
        entityActor.CM.GetComponent<Gravity>().LandedOnFloor += 
            () => CurrentJumps = 0;
        
        entityActor.CM.GetComponent<Gravity>().LeftFloor += () => {
            // If we fell
            if (CurrentJumps == 0)
            {
                CurrentJumps++;
            }
        };
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
        entityActor.velocity.Y = Mathf.Clamp(entityActor.velocity.Y, 0, MathT.POSITIVE_INF);
    }

    public void LimitJump()
    {
        if (entityActor.velocity.Y <= -LimitThreshold)
        {
            entityActor.velocity.Y += LimitThreshold;
        }
    }

    private void executeJump(float forceMultiplier=1f)
    {
        entityActor.velocity.Y = -jumpVelocity*forceMultiplier;
        CurrentJumps++;
    }

    private bool canJump()
    {
        return CurrentJumps < MaxJumps;
    }
    
}