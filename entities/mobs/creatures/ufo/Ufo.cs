using Godot;
using System;
using TENamespace.basic.builders.gravity_bullet_spawner;
using TerraEngineer;
using TerraEngineer.entities.mobs.creatures;

public partial class Ufo : Creature
{
    public Vector2 FlyAroundPoint;
    [Export] public float MarginAroundPoint = 20f;
    [Export] private float timeUntilBored = 5f;
    
    private readonly Vector2 distanceToKeepFromPlayer = new Vector2(0, -50);
    private Player player;
    
    private readonly BombardState bombardState = new();
    private readonly IdleState idleState = new();
    
    private StateMachineWithTriggers<Ufo,GenericCreatureTriggers> fsm;
    
    public override void Init()
    {
        player = GetNode<Player>(Names.NodePaths.Player);
        
        FlyAroundPoint = GlobalPosition;
        fsm = new StateMachineWithTriggers<Ufo, GenericCreatureTriggers>(this, idleState);
        fsm.AddTransition(idleState, bombardState, (() => fsm.IsTriggered(GenericCreatureTriggers.EnemyDetected)));
        fsm.AddTransition(bombardState, idleState, (() => fsm.IsTriggered(GenericCreatureTriggers.EnemyLost)));
    }
    
    public override void _PhysicsProcess(double delta)
    {
        fsm.Update((float)delta);
        CM.UpdateComponents((float)delta);
        
        HandleMove();
        FlipIfHitWall();
    }
    
    // TODO - This is copied over from Moth. Try to do it without repetition later
    // Add this funcionality to free fly compoentn?
    public class BombardState : State<Ufo>
    {
        private ITimer bombardTimer;
        
        public override void Enter()
        {
            bombardTimer = TimerManager.Schedule(2.5f, true, Actor, _ =>
            {
                Actor.CM.GetComponent<GravityBulletSpawner>()
                    .Start()
                    .SetPosition(Actor.GlobalPosition)
                    .Build();

                Actor.CM.GetComponent<GravityBulletSpawner>().AddToGame();
            });
        }

        public override void Update(float dt)
        {
            Actor.CM.GetComponent<FreeFly>().FlyToPoint(Actor.player.GlobalPosition + Actor.distanceToKeepFromPlayer, dt); 
        }

        public override void Exit()
        {
            bombardTimer?.Stop();
        }
    }
    
    public class IdleState : State<Ufo>
    {
        private Vector2 goToPoint;
        private float delayAtPoint = 1f; //Currently delay before changing points, perhaps fix/change

        public override void Enter()
        {
            void RerollPoint(ITimer timer)
            {
                if(!IsInstanceValid(Actor)) return;
                Vector2 rnd = MathT.RandomVector2(-Actor.MarginAroundPoint, Actor.MarginAroundPoint);
                goToPoint = Actor.FlyAroundPoint + rnd;
                TimerManager.Schedule(delayAtPoint, Actor,  RerollPoint);
            }
            RerollPoint(null);
        }

        public override void Update(float dt)
        {
            Actor.CM.GetComponent<FreeFly>().FlyToPoint(goToPoint, dt);    
        }
    }
    
    private bool seesPlayer = false;
    private void onDetectionAreaBodyEntered(Node2D body)
    {
        fsm.FireTrigger(GenericCreatureTriggers.EnemyDetected);
    }
    
    private void onDetectionAreaBodyExited(Node2D body)
    {
        seesPlayer = false;
        TimerManager.Schedule(timeUntilBored, this, (_) =>
        {
            if (!seesPlayer)
            {
                fsm.FireTrigger(GenericCreatureTriggers.EnemyLost);
            }
        });
    }

}
