using Godot;
using TerraEngineer.entities.mobs;

namespace TENamespace.lifetime;

public partial class Lifetime : Component
{
    [Export] private Entity actor;
    [Export] private float lifetime = 100;

    private ITimer timer;
    
    public override void _Ready()
    {
        timer = TimerManager.Schedule(lifetime, this, (t) =>
        {
             actor.Die();
        });
    }
    
}