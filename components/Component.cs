using Godot;
using TerraEngineer.entities.mobs;

namespace TENamespace;

public partial class Component : Node2D
{
    protected Mob Actor;

    public virtual void Init(Mob actor)
    {
        this.Actor = actor;
    }
    
    public virtual void Update(float delta) {}

}