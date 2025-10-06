using Godot;
using System;
using TENamespace;
using TerraEngineer;
using TerraEngineer.entities.mobs;

public partial class Moth : Mob
{
    [Export] private Node2D flyAroundPoint;
    private readonly ChaseState chaseState = new();
    private readonly IdleState idleState = new();
    
    private StateMachine<Moth> fsm;
    
    public override void _Ready()
    {
        fsm = new StateMachine<Moth>(this, idleState);
        // fsm.AddTransition(chaseState, waitState, chaseState.LandedOnFloor);
        // fsm.AddTransition(waitState, jumpState, idleState.TimerCondition);
    }

    public override void _PhysicsProcess(double delta)
    {
        fsm.Update((float)delta);

        Velocity = velocity;
        MoveAndSlide();
    }

    public class ChaseState : IState<Moth>
    {
        public Func<bool> LandedOnFloor => () => isLandingOnFloor;
        private bool isLandingOnFloor = false;
        private void landedOnFloor() => isLandingOnFloor = true;
        
        public void Enter(Moth actor)
        {
        }
        
        public void Update(Moth actor, float dt)
        {
        }
        
        public void Exit(Moth actor)
        {
        }
        
        // Stop chase if long delay and player not in detection range
    }
    
    public class IdleState : IState<Moth>
    {
        public void Enter(Moth actor)
        {
            
        }

        public void Update(Moth actor, float dt)
        {
            
        }

        public void Exit(Moth actor)
        {
            
        }
        
        // Start chase if player in detection range
    }
    
    private void onDetectionAreaBodyEntered(Node2D body)
    {
        
    }
}
