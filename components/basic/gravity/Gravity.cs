using System;
using Godot;
using TerraEngineer;
using TerraEngineer.entities.mobs;

namespace TENamespace;

public partial class Gravity : Component
{
    [Export] public float GravityForce = 120f;
    [Export] private float maxGravity = 600f;
    
    public bool Disabled = false;
    
    private bool isOnFloor = false;
    public delegate void LandedOnFloorHandler();
    public event LandedOnFloorHandler LandedOnFloor;
    
    public delegate void LeftFloorHandler();
    public event LandedOnFloorHandler LeftFloor;

    private Entity entityActor;

    public override void Init(Node2D actor)
    {
        base.Init(actor);
        if (actor is Entity entity)
        {
            entityActor = entity;
        }
        else
        {
            throw new Exception("Gravity component requires Entity actor.");
        }
    }

    public override void Update(float delta)
    {
        if(Disabled) return;
        
        if (entityActor.IsOnFloor())
        {
            if (entityActor.UpDirection.Y != 0)
            {
                entityActor.velocity.Y = Mathf.Clamp(entityActor.velocity.Y, 
                    entityActor.UpDirection.Y < 0 ? MathT.NEGATIVE_INF : 0,
                    entityActor.UpDirection.Y < 0 ? 0 : MathT.POSITIVE_INF);
            }

            if (entityActor.UpDirection.X != 0)
            {
                entityActor.velocity.X = Mathf.Clamp(entityActor.velocity.X, 
                    entityActor.UpDirection.X < 0 ? 0 : MathT.NEGATIVE_INF,
                    entityActor.UpDirection.X < 0 ? MathT.POSITIVE_INF : 0);    
            }
        }
        else
        {
            if (entityActor.velocity.Y * -entityActor.UpDirection.Y < maxGravity * Math.Abs(entityActor.UpDirection.Y))
            {
                entityActor.velocity.Y += GravityForce * -entityActor.UpDirection.Y * delta;    
            }   
            
            if (entityActor.velocity.X * entityActor.UpDirection.X < maxGravity * Math.Abs(entityActor.UpDirection.X))
            {
                entityActor.velocity.X += GravityForce * -entityActor.UpDirection.X;    
            }   
        }
        
        checkEvents();   
    }
    
    private void checkEvents()
    {
        if (!entityActor.IsOnFloor())
        {
            if (isOnFloor)
            {
                isOnFloor = false;
                LeftFloor?.Invoke();
            }
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