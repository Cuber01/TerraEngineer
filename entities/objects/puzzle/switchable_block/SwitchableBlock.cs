using System.Collections.Generic;
using Godot;
using Godot.Collections;
using TerraEngineer.entities.objects.puzzle;

namespace TerraEngineer.entities.tiles.switchable_tile;

public partial class SwitchableBlock : Node2D, ISwitchableDependent
{
    public AnimatedSprite2D Sprite { get; set;  }

    public void OnSwitch(bool switchedOn)
    {
        GD.Print("Switched " + Name + " to value " + switchedOn);
    }
}