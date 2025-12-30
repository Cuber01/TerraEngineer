using Godot;
using TerraEngineer.entities.mobs;

namespace TENamespace;

public partial class Component : Node2D
{

    protected Entity Actor;

    public virtual void Init(Entity actor)
    {
        this.Actor = actor;
    }
    
    // Has to be called by the actor manually
    public virtual void OptionalInit(Entity actor) { }
    
    public virtual void Update(float delta) {}

}