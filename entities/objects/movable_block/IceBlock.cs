using Godot;

namespace TerraEngineer.entities.objects.movable_block;

public partial class IceBlock : MovableBlock
{
    protected override void HandleVelocity(float delta)
    {
        if (IsPushed && PushingSpeed > VelocityNeededToPush)
        {
            velocity.X = PushingSpeed * PushDirection;
        }
    }
}