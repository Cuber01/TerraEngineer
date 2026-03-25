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
    [Export] private float limitThreshold = 30f;
    
    private int currentJumps = 0;
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
            () => currentJumps = 0;
        
        entityActor.CM.GetComponent<Gravity>().LeftFloor += () => {
            // If we fell
            if (currentJumps == 0)
            {
                currentJumps++;
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
        if (entityActor.velocity.Y <= -limitThreshold)
        {
            entityActor.velocity.Y += limitThreshold;
        }
    }

    private void executeJump(float forceMultiplier=1f)
    {
        entityActor.velocity.Y = -jumpVelocity*forceMultiplier;
        currentJumps++;
    }

    private bool canJump()
    {
        return currentJumps < MaxJumps;
    }
    
}