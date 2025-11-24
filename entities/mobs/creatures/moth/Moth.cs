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

    public class ChaseState : IState<Moth>
    {
        public Entity ChaseTarget { set; private get; }
        
        public void Enter(Moth actor)
        {
        }
        
        public void Update(Moth actor, float dt)
        {
            actor.CM.GetComponent<FreeFly>().FlyToPoint(ChaseTarget.GlobalPosition, dt);  
        }
        
        public void Exit(Moth actor) { }

        
        // Stop chase if long delay and player not in detection range
    }
    
    public class IdleState : IState<Moth>
    {
        private Vector2 goToPoint;
        private float delayAtPoint = 1f; //Currently delay before changing points, perhaps fix/change

        public void Enter(Moth actor)
        {
            void RerollPoint(ITimer timer)
            {
                if(!IsInstanceValid(actor)) return;
                Vector2 rnd = MathT.RandomVector2(-actor.MarginAroundPoint, actor.MarginAroundPoint);
                goToPoint = actor.FlyAroundPoint.GlobalPosition + rnd;
                TimerManager.Schedule(delayAtPoint, actor,  RerollPoint);
            }
            RerollPoint(null);
        }

        public void Update(Moth actor, float dt)
        {
           actor.CM.GetComponent<FreeFly>().FlyToPoint(goToPoint, dt);    
        }

        public void Exit(Moth actor)
        {
            
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
        TimerManager.Schedule(timeUntilBored, this, (t) =>
        {
            if (!seesPlayer)
            {
                fsm.ChangeState(idleState);
            }
        });
    }
    
    
}
