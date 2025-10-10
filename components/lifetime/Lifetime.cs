using Godot;
using TerraEngineer.entities.mobs;

namespace TENamespace.lifetime;

public partial class Lifetime : Component
{
    [Export] private Mob actor;
    [Export] private float lifetime;
    
    public override void _Ready()
    {
        TimerManager.Schedule(lifetime, (t) =>
        {
            actor.Die();
        });
    }
}