using Godot;
using System;
using TerraEngineer.entities.mobs;

namespace TENamespace;

public partial class Dash : Component
{
    [ExportGroup("External")]
    
    [Export] private float dashSpeed = 400;
    [Export] private float dashDuration = 0.06f;
    [Export] private int maxDashes = 1;

    private int currentDashes = 0;
    public bool IsDashing = false;
    private int dashDirection;
    
    private Entity entityActor;
    private Gravity entityGravity;

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
        
        entityGravity = entityActor.CM.GetComponent<Gravity>();
        entityGravity.LandedOnFloor += () => currentDashes = 0;
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
        entityGravity.Disabled = true;
        currentDashes++;
    }

    private void endDash(ITimer timer)
    {
        IsDashing = false;
        entityGravity.Disabled = false;
    }

    private bool canDash()
    {
        return currentDashes < maxDashes;
    }
    
}