using Godot;

namespace TENamespace;

public partial class Gravity : Component
{
    [Export] private float gravityForce = 2f;
    [Export] private float maxGravity = 60f;
    
    public void ApplyGravity(float delta)
    {
        if (Actor.velocity.Y < maxGravity)
        {
            Actor.velocity.Y += gravityForce;    
        }
    }
}