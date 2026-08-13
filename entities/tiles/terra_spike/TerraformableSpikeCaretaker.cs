using Godot;
using TerraEngineer.entities.mobs;

namespace TerraEngineer.entities.tiles.terra_spike;

public partial class TerraformableSpikeCaretaker : TerraformableTileCaretaker
{
    [Export] private RayCast2D groundLeft;
    [Export] private RayCast2D groundRight;
    [Export] private RayCast2D groundDown;
    [Export] private RayCast2D groundUp;
    public Direction4 SpikeFacing = Direction4.None;

    public override void _Ready()
    {
        RunDisable();
    }

    private void init()
    {
        base._Ready();
        
        // We don't need them anymore
        groundUp.SetDeferred(RayCast2D.PropertyName.Enabled, false);
        groundDown.SetDeferred(RayCast2D.PropertyName.Enabled, false);
        groundLeft.SetDeferred(RayCast2D.PropertyName.Enabled, false);
        groundRight.SetDeferred(RayCast2D.PropertyName.Enabled, false);    
    }
    
    public override void _PhysicsProcess(double delta)
    {
        // It can take some time for ground hitboxes to init,
        // so we search for collision until we find them
        if(SpikeFacing != Direction4.None) return;

        if (groundUp.IsColliding())
        {
            SpikeFacing = Direction4.Down;
        }else if (groundDown.IsColliding()) {
                SpikeFacing = Direction4.Up;
        } else if (groundLeft.IsColliding()) {
            SpikeFacing = Direction4.Right;
        } else if (groundRight.IsColliding()) {
            SpikeFacing = Direction4.Left;
        } 

        if (SpikeFacing != Direction4.None)
        {
            // Init only once we're ready
            init();
        }
    }
}