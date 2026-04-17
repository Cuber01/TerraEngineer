using Godot;
using System;
using TENamespace.basic.builders.gravity_bullet_spawner;
using TerraEngineer;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.mobs.creatures;

public partial class Ufo : FlyingCreature
{
    private readonly Vector2 distanceToKeepFromPlayer = new Vector2(0, -50);
    
    private readonly BombardState bombardState = new();
    private readonly FlyingIdleState<FlyingCreature> idleState = new();
    
    public override void Init()
    {
        Player = GetNode<Player>(Names.NodePaths.Player);
        
        FlyAroundPoint = GlobalPosition;
        fsm = new StateMachineWithTriggers<FlyingCreature, GenericCreatureTriggers>(this, idleState);
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
    
    public class BombardState : State<FlyingCreature>
    {
        private Action<ITimer> shootAction;
        private ITimer shootTimer;

        public override void Enter()
        {
            Ufo ufo = (Ufo)Actor;
            shootAction = _ =>
            {
                ufo.CM.GetComponent<GravityBulletSpawner>()
                    .Start()
                    .SetPosition(ufo.GlobalPosition)
                    .Build();

                ufo.CM.GetComponent<GravityBulletSpawner>().AddToGame();
            };
            shootTimer = TimerManager.Schedule(2.5f, true, ufo, shootAction);
        }

        public override void Exit()
        {
            shootTimer?.Stop();
        }

        public override void Update(float dt)
        {
            Ufo ufo = (Ufo)Actor;
            Vector2 targetPoint = ufo.Player.GlobalPosition + ufo.distanceToKeepFromPlayer;
            ufo.CM.GetComponent<FreeFly>().FlyToPoint(targetPoint, dt);
        }
    }
    
}
