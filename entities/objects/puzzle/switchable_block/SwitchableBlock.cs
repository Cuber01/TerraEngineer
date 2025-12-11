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
    [Export] private bool DefaultState
    {
        get => defaultState;
        set
        {
            defaultState = value;
            Sprite.Frame = defaultState ? 0 : 1;
        }
    }

    private bool defaultState = true;

    public override void _Ready()
    {
        MakeShaderUnique();

        if (!defaultState)
        {
            collider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
            Sprite.Frame = 1;    
        }
    }
    
    public void OnSwitch(bool switchedOn)
    {
        collider.SetDeferred(CollisionShape2D.PropertyName.Disabled, switchedOn == defaultState);
        Sprite.Frame = (switchedOn == defaultState) ? 1 : 0;
    }
}