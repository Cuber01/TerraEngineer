using Godot;
using TerraEngineer;

namespace TENamespace.basic;

public partial class TrigFly : FreeFly
{
    [Export] private float frequency = 80.0f;
    [Export] private float amplitude = 80.0f;
    
    private float time = 0;
    
    public void FlyInDirectionSinusoidal(Vector2 directionNormal, float dt)
    {
        time += dt;
        EntityActor.velocity.X = MathT.Lerp(EntityActor.velocity.X,
            directionNormal.X * speed, acceleration, dt);
        EntityActor.velocity.Y = MathT.Lerp(EntityActor.velocity.Y,
            directionNormal.Y * speed + amplitude*Mathf.Sin(time*frequency), acceleration, dt);
    }
    
    
}