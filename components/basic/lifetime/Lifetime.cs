using Godot;
using System;
using TerraEngineer.entities.mobs;

namespace TENamespace.lifetime;

public partial class Lifetime : Component
{
    [Export] private float lifetime = 100;
    
    public delegate void LifetimeEndedHandler();
    public event LifetimeEndedHandler LifetimeEnded;
    
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
            throw new Exception("Lifetime component requires Entity actor.");
        }
    }

    public override void _Ready()
    {
        TimerManager.Schedule(lifetime, this, (t) =>
        {
             LifetimeEnded?.Invoke();
             entityActor.Die();
        });
    }
    
}