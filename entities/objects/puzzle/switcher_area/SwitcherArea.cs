using Godot;
using TerraEngineer.entities.mobs;

namespace TerraEngineer.entities.objects.puzzle;

public partial class SwitcherArea : Area2D, ISwitcher
{
    public event ISwitcher.SwitchedEventHandler Switched;
    public bool SwitchedOn { get; set; }

    [Export] private bool startState = false;

    public override void _Ready()
    {
        SwitchedOn = startState;
    }
    
    private void onPlayerEntered(Node2D body)
    {
        if (SwitchedOn == startState)
        {
            SwitchedOn = !startState;
            Switched?.Invoke(SwitchedOn);    
        }
    }
}