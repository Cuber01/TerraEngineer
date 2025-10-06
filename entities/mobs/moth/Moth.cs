using Godot;
using System;
using TENamespace;
using TerraEngineer;
using TerraEngineer.entities.mobs;


public partial class Moth : Mob
{
    [Export] public Node2D FlyAroundPoint;
    [Export] public float MarginAroundPoint = 20f;

    private readonly ChaseState chaseState = new();
    private readonly IdleState idleState = new();
    
    private StateMachine<Moth> fsm;
    
    public override void _Ready()
    {
        FlyAroundPoint.GlobalPosition = GlobalPosition;
        fsm = new StateMachine<Moth>(this, idleState);
        
        // fsm.AddTransition(chaseState, waitState, chaseState.LandedOnFloor);
        // fsm.AddTransition(waitState, jumpState, idleState.TimerCondition);
    }

    public override void _PhysicsProcess(double delta)
    {
        fsm.Update((float)delta);
        CM.UpdateComponents((float)delta);

        Velocity = velocity;
        MoveAndSlide();
    }

    public class ChaseState : IState<Moth>
    {
        public Mob ChaseTarget { set; private get; }
        
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
        private Vector2 goToPoint;
        private float delayAtPoint = 3f; //Currently delay before changing points, perhaps fix/change

        public void Enter(Moth actor)
        {
            void RerollPoint(ITimer timer)
            {
                Vector2 rnd = MathTools.RandomVector2(-actor.MarginAroundPoint, actor.MarginAroundPoint);
                goToPoint = actor.FlyAroundPoint.GlobalPosition + rnd;
                TimerManager.Schedule(delayAtPoint, RerollPoint);
            }
            RerollPoint(null);
        }

        public void Update(Moth actor, float dt)
        {
           actor.CM.GetComponent<FreeFly>().FlyToPoint(goToPoint);    
        }

        public void Exit(Moth actor)
        {
            
        }
        
        // Start chase if player in detection range
    }
    
    private void onDetectionAreaBodyEntered(Node2D body)
    {
        chaseState.ChaseTarget = (Mob)body;
    }
}
