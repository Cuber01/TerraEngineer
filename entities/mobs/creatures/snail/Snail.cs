using Godot;
using System;
using TENamespace;
using TerraEngineer;
using TerraEngineer.entities.mobs;

public partial class Snail : Mob
{
    [Export] public Direction4 CurrentRotation;
    [Export] private RayCast2D down;
    [Export] private RayCast2D right;
    [Export] private RayCast2D left;
    
    private readonly WalkState walkState = new WalkState();
    private readonly RotateState rotateState = new RotateState();
    
    private StateMachine<Snail> fsm;
    public DirectionX RotateInDirection = DirectionX.None;
    
    public override void _Ready()
    {
        fsm = new StateMachine<Snail>(this, walkState, true);
        
        fsm.AddTransition(walkState, rotateState, needsRotate);
    }

    private bool needsRotate()
    {
        bool needsRotate = false;
        
        if (down.IsColliding()) {
            if (Facing == DirectionX.Right && right.IsColliding())
            {
                RotateInDirection = DirectionX.Left;
                needsRotate = true;
            }
            
            if (Facing == DirectionX.Left && left.IsColliding())
            {
                RotateInDirection = DirectionX.Right;
                needsRotate = true;
            }
        }
        else
        {
            RotateInDirection = Facing;
            needsRotate = true;
        }
        
        return needsRotate;
    }


    public override void _PhysicsProcess(double delta)
    {
        fsm.Update((float)delta);
        CM.UpdateComponents((float)delta);
        
        Velocity = velocity;
        MoveAndSlide();
    }

    public class WalkState : IState<Snail>
    {
        public void Enter(Snail actor)
        {
            throw new NotImplementedException();
        }

        public void Update(Snail actor, float dt)
        {
            throw new NotImplementedException();
        }

        public void Exit(Snail actor)
        {
            throw new NotImplementedException();
        }

    }

    
    public class RotateState : IState<Snail>
    {
        public void Enter(Snail actor)
        {
            throw new NotImplementedException();
        }

        public void Update(Snail actor, float dt)
        {
            throw new NotImplementedException();
        }

        public void Exit(Snail actor)
        {
            throw new NotImplementedException();
        }
    }
    

}
