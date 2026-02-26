
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

    public void AddTransition(State<T> from, State<T> to, System.Func<bool> condition, float probability=1f)
    {
        Transition<T> newTransition = new Transition<T>(to, condition, from, probability); 
        localTransitions.Add(newTransition);
        if (from == CurrentState)
        {
            possibleTransitions.Add(newTransition);
        }
    }
        
    public void AddGlobalTransition(State<T> to, System.Func<bool> condition, float probability=1f) => 
        globalTransitions.Add(new Transition<T>(to, condition, null, probability));

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
            return chooseRandomTransition(availableTransitions);
        }
    }

    private Transition<T> chooseRandomTransition(List<Transition<T>> availableTransitions)
    {
        #if DEBUG
        float totalProbability = availableTransitions.Sum(t => t.Probability);
        if (totalProbability != 1f)
            throw new Exception("Summed probabilities of possible do not sum to 100%");
        #endif

        float cumulativeChance = 0f;
        float roll = MathT.RandomFloat(0,1);
        foreach (var transition in availableTransitions)
        {
            cumulativeChance += transition.Probability;
            if (roll < cumulativeChance)
            {
                return transition;
            }
        }

        throw new Exception("Couldn't find transition");
    }

    // If from=null it's a global transition
    private class Transition<U>(State<U> to, System.Func<bool> condition, State<U> from, float probability)
    {
        public readonly State<U> From = from;
        public readonly State<U> To = to;
        public readonly System.Func<bool> Condition = condition;
        public readonly float Probability = probability;
    }
    
}
