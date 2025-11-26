using Godot;

namespace TerraEngineer.entities.objects.puzzle;

public partial class Button : Node2D, ISwitcher
{
    public event ISwitcher.SwitchedEventHandler Switched;
    public bool SwitchedOn { get; set; }
    
    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("f2"))
        {
            SwitchedOn = !SwitchedOn;
            Switched?.Invoke(SwitchedOn);
        }
    }
}