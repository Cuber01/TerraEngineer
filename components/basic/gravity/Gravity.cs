using Godot;
using TerraEngineer;

namespace TENamespace;

public partial class Gravity : Component
{
    [Export] private float gravityForce = 2f;
    [Export] private float maxGravity = 100f;
    
    private bool isOnFloor = false;
    public delegate void LandedOnFloorHandler();
    public event LandedOnFloorHandler LandedOnFloor;
    
    public override void Update(float delta) => updateGravity(delta);
    
    private void updateGravity(float delta)
    {
        
        if (Actor.IsOnFloor())
        {
            if (Actor.UpDirection.Y != 0)
            {
                Actor.velocity.Y = Mathf.Clamp(Actor.velocity.Y, 
                    Actor.UpDirection.Y < 0 ? MathTools.NEGATIVE_INF : 0,
                    Actor.UpDirection.Y < 0 ? 0 : MathTools.POSITIVE_INF);
            }

            if (Actor.UpDirection.X != 0)
            {
                Actor.velocity.X = Mathf.Clamp(Actor.velocity.X, 
                    Actor.UpDirection.X < 0 ? 0 : MathTools.NEGATIVE_INF,
                    Actor.UpDirection.X < 0 ? MathTools.POSITIVE_INF : 0);    
            }
        }
        else
        {
            if (Actor.velocity.Y * -Actor.UpDirection.Y < maxGravity * -Actor.UpDirection.Y)
            {
                Actor.velocity.Y += gravityForce * -Actor.UpDirection.Y;    
            }   
            
            if (Actor.velocity.X * Actor.UpDirection.X < maxGravity * Actor.UpDirection.X)
            {
                Actor.velocity.X += gravityForce * Actor.UpDirection.X;    
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