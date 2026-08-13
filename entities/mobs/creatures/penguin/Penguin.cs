using Godot;
using System;
using TENamespace;
using TENamespace.projectile_builder;
using TerraEngineer;
using TerraEngineer.entities.mobs.creatures;

[Tool]
public partial class Penguin : Creature
{
    [Export] private CollisionShape2D slideShape;
    [Export] private CollisionShape2D idleShape;
    
    [Export] private RayCast2D isGroundAhead;
    [Export] private RayCast2D isPlayerBelow;

    private ITimer shootTimer = new QuickTimer();
    private float shootCooldown = 4f;
    private bool shootingAllowed = true;

    
    private readonly StartState startState = new StartState();
    private readonly SlidingState slidingState = new SlidingState();
    private readonly JumpState jumpState = new JumpState();

    private StateMachine<Penguin> fsm;

    public override void Init()
    {
        fsm = new StateMachine<Penguin>(this, startState);
        fsm.AddTransition(startState, slidingState, () => startState.TimerCondition());
        fsm.AddTransition(slidingState, jumpState, () => !isGroundAhead.IsColliding());
        fsm.AddTransition(jumpState, slidingState, jumpState.LandedOnFloor);
    }

    public override void _PhysicsProcess(double delta)
    {
        #if TOOLS
        if (Engine.IsEditorHint())
            return;
        #endif

        fsm.Update((float)delta);
        CM.UpdateComponents((float)delta);

        HandleMove();
        FlipIfHitWall();
    }

    private void attemptShoot()
    {
        if (shootingAllowed && isPlayerBelow.IsColliding())
        {
            // Check if there is ground between us and player
            if(MathT.CheckRaycast2D(GlobalPosition, isPlayerBelow.GetCollisionPoint(),
                   1u << (1 - 1), // Layer 1 (ground)
                   GetWorld2D().DirectSpaceState ).Count == 0)
            {
                CM.GetComponent<ProjectileSpawner>()
                    .Start()
                    .SetPosition(GlobalPosition)
                    .AddToGame()
                    .Build();
            
                TimerManager.Schedule(shootCooldown, this, 
                    (_) => shootingAllowed = true);
                shootingAllowed = false;
            }
        }
    }

    public class StartState : TimedState<Penguin>
    {
        public override void Enter()
        {
            Actor.idleShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
            Actor.slideShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
            Delay = 1f;
            base.Enter();
        }
        
        public override void Exit()
        {
            Actor.idleShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
        }
    }

    public class SlidingState : State<Penguin>
    {
        public override void Enter()
        {
            Actor.slideShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
        }

        public override void Update(float dt)
        {
            Actor.CM.GetComponent<Move>().Walk(Actor.Facing, dt);

            Actor.attemptShoot();
        }

        public override void Exit()
        {
        }
    }

    public class JumpState : State<Penguin>
    {
        public Func<bool> LandedOnFloor => () => isLandingOnFloor;
        private bool isLandingOnFloor = false;
        private void landedOnFloor() => isLandingOnFloor = true;

        public override void Enter()
        {
            isLandingOnFloor = false;
            jump();
        }

        public override void Update(float dt)
        {
            Actor.CM.GetComponent<Move>().Walk(Actor.Facing, dt);

            Actor.attemptShoot();
            
        }

        public override void Exit()
        {
            Actor.SpriteWrapper.Play(Names.Animations.Idle);
            Actor.CM.GetComponent<Gravity>().LandedOnFloor -= landedOnFloor;
        }

        private void jump()
        {
            Actor.CM.GetComponent<Jump>().AttemptJump();
            Actor.CM.GetComponent<Gravity>().LandedOnFloor += landedOnFloor;
        }
    }
    
    protected override void FlipEffect()
    {
        base.FlipEffect();
        isGroundAhead.Position = new Vector2(-isGroundAhead.Position.X, isGroundAhead.Position.Y);
    }
}
