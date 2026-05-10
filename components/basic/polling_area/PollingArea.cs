using Godot;
using System;
using System.Collections.Generic;
using TENamespace;

public partial class PollingArea : Component
{
    private List<Node2D> intersectingBodies = new();

    public bool IsColliding()
    {
        return intersectingBodies.Count > 0;
    }
    
    private void onBodyEntered(Node2D body)
    {
        intersectingBodies.Add(body);
    }

    private void onBodyExited(Node2D body)
    {
        intersectingBodies.Remove(body);
    }
}
