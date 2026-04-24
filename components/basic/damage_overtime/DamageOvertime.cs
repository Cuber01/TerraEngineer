using Godot;
using System;
using TENamespace.health;
using TerraEngineer.entities.mobs;

namespace TENamespace.basic.damage_overtime;

public partial class DamageOvertime : Component
{
    [Export] private float timeBetweenDamage = 2;
    [Export] private int damageAmount = 1;
    
    private Entity entityActor;
    private ITimer timer;

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
        timer = TimerManager.Schedule(timeBetweenDamage, true ,this, (t) =>
        {
            Health hpComponent = entityActor.CM.TryGetComponent<Health>();
            hpComponent?.ChangeHealth(-damageAmount);
        });
    }

    public override void OnRemoved()
    {
        TimerManager.Cancel(timer);
    }

}