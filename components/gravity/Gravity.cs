using Godot;

namespace TENamespace;

public partial class Gravity : Component
{
    [Export] private float gravityForce = 2f;

    public void ApplyGravity(float delta)
    {
        Actor.velocity.Y += gravityForce;
    }
}