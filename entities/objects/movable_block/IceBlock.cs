using Godot;

namespace TerraEngineer.entities.objects.movable_block;

public partial class IceBlock : MovableBlock
{
    protected override bool StopCondition()
    {
        return TestMove(GlobalTransform, velocity); // If we would collide with wall
    }
}