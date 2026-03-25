using Godot;
using System;
using TerraEngineer.entities.mobs;

namespace TENamespace;

public partial class Dash : Component
{
    [Export] private float dashSpeed = 400;
    [Export] private float dashDuration = 0.06f;
    [Export] private int maxDashes = 1;

    private int currentDashes = 0;
    public bool IsDashing = false;
    private int dashDirection;
    
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
            throw new Exception("Dash component requires Entity actor.");
        }
        
        entityActor.CM.GetComponent<Gravity>().LandedOnFloor += 
            () => currentDashes = 0;
    }
    
    public override void Update(float delta)
    {
        if (IsDashing)
        {
            entityActor.velocity.X = dashSpeed * dashDirection;
            entityActor.velocity.Y = 0;
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
        IsDashing = true;
        dashDirection = (int)direction;
        TimerManager.Schedule(dashDuration,  this, endDash);
        currentDashes++;
    }

    private void endDash(ITimer timer)
    {
        IsDashing = false;
    }

    private bool canDash()
    {
        return currentDashes < maxDashes;
    }
    
}