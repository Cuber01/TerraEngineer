using Godot;

namespace TENamespace.basic.orbit;

public partial class Orbit : FreeFly
{
    [Export] private float orbitRadius = 50f;
    [Export] private float angularVelocity = 1.5f; // radians per second
    
    private Vector2 orbitCenter;
    private float currentAngle = 0f;
    
    public void OrbitAround(Vector2 centerPoint, float dt)
    {
        orbitCenter = centerPoint;
        currentAngle += angularVelocity * dt;
        
        Vector2 targetPosition = orbitCenter + new Vector2(
            Mathf.Cos(currentAngle) * orbitRadius,
            Mathf.Sin(currentAngle) * orbitRadius
        );
        
        FlyToPoint(targetPosition, dt);
    }
}