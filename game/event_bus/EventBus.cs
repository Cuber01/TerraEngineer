using Godot;
using System;
using System.Collections.Generic;
using System.Security;

namespace TerraEngineer.game;

public class EventBus<T> where T : Enum
{
    protected Dictionary<T, List<Action>> Subs = new();

    public void Subscribe(T eventType, Action handler)
    {
        if (!Subs.ContainsKey(eventType))
            Subs[eventType] = new List<Action>();
        
        Subs[eventType].Add(handler);
    }

    public void Unsubscribe(T eventType, Action handler)
    {
        Subs[eventType].Remove(handler);
    }

    public void Publish(T eventType)
    {
        if (Subs.TryGetValue(eventType, out List<Action> subs))
        {
            foreach (Action sub in subs)
            {
                sub.Invoke();
            }
        }
    }

}