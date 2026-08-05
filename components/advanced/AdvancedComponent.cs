using Godot;

namespace TENamespace.advanced;

// Advanced components have sub-components
public partial class AdvancedComponent : Component
{
    [Export] public ComponentManager CM;
    
    public override void Init(Node2D actor)
    {
        this.Actor = actor;
        CM.InitComponents(actor);
    }
    
}