using Godot;
using System;

namespace TENamespace;

public partial class Component : Node2D
{

    protected Node2D Actor;

    public virtual void Init(Node2D actor)
    {
        this.Actor = actor;
    }
    
    // Has to be called by the actor manually
    public virtual void OptionalInit(Node2D actor) { }
    
    public virtual void Update(float delta) {}
    
    public virtual void OnRemoved() {}

}