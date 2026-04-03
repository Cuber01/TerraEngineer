using Godot;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.objects.puzzle;

namespace TerraEngineer.entities.tiles.switchable_tile;

[Tool]
public partial class SwitchableBlock : Entity, ISwitchable
{
    [Export] private CollisionShape2D collider;
    [Export] private bool DefaultState
    {
        get => _defaultState;
        set
        {
            _defaultState = value;
            if(SpriteWrapper.Initialized) // Skip before _Ready
                SpriteWrapper.SetFrame(_defaultState ? 0 : 1);
        }
    }
    private bool _defaultState = true;

    public override void _Ready()
    {
        MakeShaderUnique();
        InitSpriteWrapper();

        if (!_defaultState)
        {
            collider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
            SpriteWrapper.SetFrame(1);    
        }
    }
    
    public void OnSwitch(bool switchedOn)
    {
        collider.SetDeferred(CollisionShape2D.PropertyName.Disabled, switchedOn == _defaultState);
        SpriteWrapper.SetFrame((switchedOn == _defaultState) ? 1 : 0);
    }
}