using Godot;
using TENamespace.basic.save_tile;
using TerraEngineer.entities.tiles;

public partial class FragileTileForever : Node2D, ITile
{
    public Vector2I MapCoords { get; set; }
    [Export] private Area2D stepDetector;

    private float shatterTime = 1.5f;
    private float repairTime = 3f;

    
    public override void _Ready()
    {
        stepDetector.BodyEntered += steppedOn;
    }

    private void steppedOn(Node2D stepper)
    {
        TimerManager.Schedule(shatterTime, this, shatter);
    }
    
    private void shatter(ITimer _)
    {
        QueueFree();
    }

}