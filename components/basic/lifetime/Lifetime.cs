using Godot;
using TerraEngineer.entities.mobs;

namespace TENamespace.lifetime;

public partial class Lifetime : Component
{
    [Export] private Entity actor;
    [Export] private float lifetime = 100;
    
    public override void _Ready()
    {
        TimerManager.Schedule(lifetime, (t) =>
        {
            actor.Die();
        });
    }
}