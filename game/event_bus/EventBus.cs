using Godot;
using System;
using System.Collections.Generic;
using System.Security;

namespace TerraEngineer.game;

public class EventBus<T> where T : Enum
{
    protected Dictionary<T, Action> Subs = new Dictionary<T, Action>();

    public void Subscribe(T eventType, Action handler)
    {
        if (!Subs.ContainsKey(eventType))
            Subs[eventType] = null;
        
        Subs[eventType] += handler;
    }

    public void Unsubscribe(T eventType, Action handler)
    {
        Subs[eventType] -= handler;
    }

    public void Publish(T eventType)
    {
        Subs[eventType]?.Invoke();
    }

}