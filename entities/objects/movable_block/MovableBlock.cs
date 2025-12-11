using Godot;
using System;
using TENamespace;
using TerraEngineer;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.mobs.creatures;

public partial class MovableBlock : Entity
{
    [Export] private float pushingSpeed = 20;
    
    bool isPushed = false;
    
    public override void _Ready()
    {
        
    }

    public override void _PhysicsProcess(double delta)
    {
        isPushed = false;
        
        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            KinematicCollision2D col = GetSlideCollision(i);
            GodotObject collidingWith = col.GetCollider();
            if (collidingWith is Player)
            {
                Vector2 normal = col.GetNormal();
                if (normal.Y == 0)
                {
                    velocity.X = pushingSpeed * Mathf.Sign(normal.X);
                    isPushed = true;
                }    
            }
        }
        
        CM.UpdateComponents((float)delta);
        
        if (!isPushed)
        {
            velocity.X = 0;
        }
        
        if (velocity.X == 0 || !TestMove(Transform, velocity*(float)delta))
        {
            Velocity = velocity;
            MoveAndSlide();    
        }
    }
}
