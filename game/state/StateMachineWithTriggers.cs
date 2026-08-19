using System;
using System.Collections.Generic;

namespace TerraEngineer;

public class StateMachineWithTriggers<T, TEnum> : StateMachine<T> where TEnum : struct, Enum 
{
    private Dictionary<TEnum, bool> triggers = new Dictionary<TEnum, bool>();

    public StateMachineWithTriggers(T actor, State<T> initialState, bool manualTransitionAllowed = false) : base(actor, initialState,
        manualTransitionAllowed)
    {
        foreach (TEnum type in Enum.GetValues<TEnum>())
        {
            triggers.Add(type, false);
        }
    }

    public bool ConsumeTrigger(TEnum trigger)
    {
        if (triggers[trigger])
        {
            triggers[trigger] = false;
            return true;
        }
        return false;
    }

    public bool ConsumeTriggers(TEnum[] list)
    {
        foreach(TEnum trigger in list)
        {
            if (!triggers[trigger])
            {
                return false;
            }
        }
        
        // We consume only if all are true
        foreach(TEnum trigger in list)
        {
            triggers[trigger] = false;
        }
        return true;
    }

    public void ResetTriggers(TEnum[] list)
    {
        foreach(TEnum trigger in list)
        {
            triggers[trigger] = false;
        }
    }
    
    public bool IsTriggered(TEnum trigger)
    {
        return triggers[trigger];
    }
    
    public void FireTrigger(TEnum trigger)
    {
        triggers[trigger] = true;
    }
}