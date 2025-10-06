using System.Collections.Generic;
using System.Linq;
using Godot;

namespace TerraEngineer;

public interface IState<T>
{
    public void Enter(T actor);
    public void Update(T actor, float dt);
    public void Exit(T actor);
}

public class TimedState<T> : IState<T>
{
    public float Time = 0;
    protected float Delay = -1;

    public virtual void Enter(T actor)
    {
        Time = 0;
    }

    public virtual void Update(T actor, float dt) {
        Time += dt;
    }
    public virtual void Exit(T actor) { }
    
    public bool TimerCondition()
    {
        return (Time >= Delay);
    }
}

