using Godot;

namespace TENamespace;

public partial class Gravity : Component
{
    [Export] private float gravityForce = 2f;
    [Export] private float maxGravity = 60f;
    
    private bool isOnFloor = false;
    public delegate void LandedOnFloorEventHandler();
    public event LandedOnFloorEventHandler LandedOnFloor;
    
    public void UpdateGravity(float delta)
    {
        if (Actor.IsOnFloor())
        {
            Actor.velocity.Y = Mathf.Clamp(Actor.velocity.Y, -999999999, 0);
        }
        else
        {
            if (Actor.velocity.Y < maxGravity)
            {
                Actor.velocity.Y += gravityForce;    
            }    
        }
        
        checkLandedOnFloor();
    }
    
    private void checkLandedOnFloor()
    {
        if (!Actor.IsOnFloor())
        {
            isOnFloor = false;
        }
        else
        {
            if (!isOnFloor)
            {
                isOnFloor = true;
                LandedOnFloor?.Invoke();
            }
        }   
    }
}