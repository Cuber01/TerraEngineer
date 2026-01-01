using System.Collections.Generic;
using System.Linq;
using Godot;

namespace TerraEngineer;

public class State<T>
{
    protected T Actor;

    public void Assign(T actor)
    {
        this.Actor = actor;
    }
    
    public virtual void Enter() {}
    public virtual void Update(float dt) {}
    public virtual void Exit() {}
}

public class TimedState<T> : State<T>
{
    public float Time = 0;
    protected float Delay = -1;

    public override void Enter()
    {
        Time = 0;
    }

    public override void Update(float dt) {
        Time += dt;
    }
    
    public bool TimerCondition()
    {
        return (Time >= Delay);
    }
}

