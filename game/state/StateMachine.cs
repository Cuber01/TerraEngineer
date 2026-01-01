
using System;
using System.Collections.Generic;
using System.Linq;
using TerraEngineer;

public class StateMachine<T>
{
    private readonly T actor;
    public State<T> CurrentState { get; private set; }
    
    private readonly List<Transition<T>> localTransitions = new();
    private List<Transition<T>> possibleTransitions = new();
    private readonly List<Transition<T>> globalTransitions = new();

    private readonly bool manualTransitionAllowed = false;
    
    public StateMachine(T actor, State<T> initialState, bool manualTransitionAllowed = false)
    {
        this.actor = actor;
        this.manualTransitionAllowed = manualTransitionAllowed;
        CurrentState = initialState;
        CurrentState.Assign(actor);
        CurrentState.Enter();
    }

    public void AddTransition(State<T> from, State<T> to, System.Func<bool> condition)
    {
        Transition<T> newTransition = new Transition<T>(to, condition, from); 
        localTransitions.Add(newTransition);
        if (from == CurrentState)
        {
            possibleTransitions.Add(newTransition);
        }
    }
        
    public void AddGlobalTransition(State<T> to, System.Func<bool> condition) => 
        globalTransitions.Add(new Transition<T>(to, condition, null));

    public void Update(float dt)
    {
        Transition<T> transition = GetTransition();
        if (transition != null) {
            changeState(transition.To);
        }
        
        CurrentState.Update(dt);
    }

    public void ChangeState(State<T> newState)
    {
        if (!manualTransitionAllowed) {
            throw new AccessViolationException("Cannot manually change state when manualTransition flag is false");
        }

        if (CurrentState == newState)
        {
            return;
        }
    
        changeState(newState);
    }
    
    private void changeState(State<T> newState)
    {
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Assign(actor);
        CurrentState.Enter();

        calculatePossibleTransitions();
    }
    
    private void calculatePossibleTransitions() =>  possibleTransitions = localTransitions
        .Where(t => t.From == CurrentState)
        .ToList();

    private Transition<T> GetTransition()
    {
        List<Transition<T>> availableTransitions = new();
        
        // Search global transitions
        foreach (var transition in globalTransitions)
        {
            if (transition.Condition())
                availableTransitions.Add(transition);
        }
        
        foreach (var transition in possibleTransitions)
        {
            if (transition.Condition())
                availableTransitions.Add(transition);
        }

        if (availableTransitions.Count == 0)
            return null;
        else if (availableTransitions.Count == 1)
            return availableTransitions[0];
        else
        {
            int index = MathT.RandomInt(0, availableTransitions.Count - 1);
            return availableTransitions[index];
        }
    }

    // If from=null it's a global transition
    private class Transition<U>(State<U> to, System.Func<bool> condition, State<U> from)
    {
        public readonly State<U> From = from;
        public readonly State<U> To = to;
        public readonly System.Func<bool> Condition = condition;
    }
    
}
