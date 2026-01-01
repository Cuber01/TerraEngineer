using Godot;
using System;
using TENamespace;
using TerraEngineer;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.mobs.creatures;


public partial class Moth : Creature
{
    [Export] public Node2D FlyAroundPoint;
    [Export] public float MarginAroundPoint = 20f;
    [Export] private float timeUntilBored = 5f;

    private readonly ChaseState chaseState = new();
    private readonly IdleState idleState = new();

    private bool seesPlayer = false;

    private StateMachine<Moth> fsm;
    
    public override void Init()
    {
        FlyAroundPoint.GlobalPosition = GlobalPosition;
        fsm = new StateMachine<Moth>(this, idleState, true);
    }

    public override void _PhysicsProcess(double delta)
    {
        fsm.Update((float)delta);
        CM.UpdateComponents((float)delta);

        Velocity = velocity;
        MoveAndSlide();
    }

    public class ChaseState : State<Moth>
    {
        public Entity ChaseTarget { set; private get; }
        
        public override void Enter()
        {
        }
        
        public override void Update( float dt)
        {
            Actor.CM.GetComponent<FreeFly>().FlyToPoint(ChaseTarget.GlobalPosition, dt);  
        }
        

        
        // Stop chase if long delay and player not in detection range
    }
    
    public class IdleState : State<Moth>
    {
        private Vector2 goToPoint;
        private float delayAtPoint = 1f; //Currently delay before changing points, perhaps fix/change

        public override void Enter()
        {
            void RerollPoint(ITimer timer)
            {
                if(!IsInstanceValid(Actor)) return;
                Vector2 rnd = MathT.RandomVector2(-Actor.MarginAroundPoint, Actor.MarginAroundPoint);
                goToPoint = Actor.FlyAroundPoint.GlobalPosition + rnd;
                TimerManager.Schedule(delayAtPoint, Actor,  RerollPoint);
            }
            RerollPoint(null);
        }

        public override void Update(float dt)
        {
           Actor.CM.GetComponent<FreeFly>().FlyToPoint(goToPoint, dt);    
        }

        
        // Start chase if player in detection range
    }
    
    private void onDetectionAreaBodyEntered(Node2D body)
    {
        chaseState.ChaseTarget = (Entity)body;
        seesPlayer = true;
        fsm.ChangeState(chaseState);
    }
    
    private void onDetectionAreaBodyExited(Node2D body)
    {
        seesPlayer = false;
        TimerManager.Schedule(timeUntilBored, this, (_) =>
        {
            if (!seesPlayer)
            {
                fsm.ChangeState(idleState);
            }
        });
    }
    
    
}
