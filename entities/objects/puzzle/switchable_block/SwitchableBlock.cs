using System.Collections.Generic;
using Godot;
using Godot.Collections;
using TerraEngineer.entities.objects.puzzle;

namespace TerraEngineer.entities.tiles.switchable_tile;

public partial class SwitchableBlock : CharacterBody2D, ISwitchableDependent
{
    [Export] private CollisionShape2D collider;
    
    public AnimatedSprite2D Sprite { get; set; }

    public void OnSwitch(bool switchedOn)
    {
        collider.SetDeferred(CollisionShape2D.PropertyName.Disabled, !switchedOn);
        Sprite.Frame = switchedOn ? 0 : 1;
    }
}