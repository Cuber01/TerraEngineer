using System.Collections.Generic;
using Godot;
using Godot.Collections;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.objects.puzzle;

namespace TerraEngineer.entities.tiles.switchable_tile;

[Tool]
public partial class SwitchableBlock : Entity, ISwitchableDependent
{
    [Export] private CollisionShape2D collider;

    public override void _Ready()
    {
        MakeShaderUnique();
    }
    
    public void OnSwitch(bool switchedOn)
    {
        collider.SetDeferred(CollisionShape2D.PropertyName.Disabled, switchedOn);
        Sprite.Frame = switchedOn ? 1 : 0;
    }
}