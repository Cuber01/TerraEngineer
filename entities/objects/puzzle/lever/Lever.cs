using Godot;
using System;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.objects.puzzle;

public partial class Lever : Entity, ISwitcher
{
    public event ISwitcher.SwitchedEventHandler Switched;
    public bool SwitchedOn { get; set; }

    private void getSwitched()
    {
        Sprite.Frame = SwitchedOn ? 0 : 1;
        SwitchedOn = !SwitchedOn;
        Switched?.Invoke(SwitchedOn);
    }
    
    private void onPlayerEntered(Player player)
    {
        player.Interacted += getSwitched;
    }
    
    private void onPlayerExited(Player player)
    {
        player.Interacted -= getSwitched;
    }

}
