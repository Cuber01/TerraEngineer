using Godot;
using System;
using TENamespace;
using TENamespace.projectile_builder;
using TENamespace.save_entity;
using TerraEngineer;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.mobs.creatures;
using TerraEngineer.game;

[Tool]
public partial class MushroomSniper : Creature
{
    private StateMachine<MushroomSniper> movementFsm;
    private StateMachine<MushroomSniper> gunFsm;
    private bool publishedEntered;

    private readonly FlankingState flankingState = new();
    private readonly RepositionState repositionState = new();
    private readonly IdleState idleState = new();
    private readonly ShootState shootState = new();
    private readonly ShootingIdleState shootingIdleState = new();
    
    public Player Player { get; private set; }

    public override void Init()
    {
        Player = GetNode<Player>(Names.NodePaths.Player);

        // --- MOVEMENT FSM ---
        movementFsm = new StateMachine<MushroomSniper>(this, idleState);
        movementFsm.AddTransition(idleState, repositionState, () => !IsPlayerAtOrAbove() && IsPlayerFarAway());
        movementFsm.AddTransition(repositionState, idleState, () => !IsPlayerAtOrAbove() && repositionState.Finished);
        
        // If player got to us, we force the reposition state ignoring previous ones and never come back
        movementFsm.AddTransition(idleState, flankingState, IsPlayerAtOrAbove);
        movementFsm.AddTransition(repositionState, flankingState, IsPlayerAtOrAbove); 

        // --- GUN FSM ---
        gunFsm = new StateMachine<MushroomSniper>(this, shootingIdleState);
        gunFsm.AddTransition(shootingIdleState, shootState, shootingIdleState.TimerCondition);
        gunFsm.AddTransition(shootState, shootingIdleState, () => shootState.Finished);

        CM.GetComponent<SaveEntity>().OptionalInit(this);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Engine.IsEditorHint())
            return;

        if (!publishedEntered)
        {
            GlobalEventBus.Instance.Publish(GlobalEvents.BossEntered);
            publishedEntered = true;
        }

        movementFsm.Update((float)delta);
        gunFsm.Update((float)delta);
        CM.UpdateComponents((float)delta);

        HandleMove();
        FlipIfHitWall();
    }

    #region Movement
    public class FlankingState : State<MushroomSniper>
    {
        private const float CloseDistanceX = 80f;
        private const float FarDistanceX = 80f;
        
        public override void Enter()
        {
            
        }

        public override void Update(float dt)
        {
            float deltaX = Actor.DeltaToPlayerX();
            float distX = Actor.DistanceToPlayerX();

            DirectionX moveDir = DirectionX.None;

            // Move away
            if (distX <= CloseDistanceX)
            {
                moveDir = deltaX > 0f ? DirectionX.Left : DirectionX.Right;
            }
            // Move towards
            else if (distX >= FarDistanceX)
            {
                moveDir = deltaX > 0f ? DirectionX.Right : DirectionX.Left;
            }

            if (moveDir != DirectionX.None)
            {
                Actor.Flip(moveDir);
                Actor.CM.GetComponent<Move>().Walk(moveDir, dt);
            }
        }

        public override void Exit()
        {
            Actor.velocity.X = 0f;
        }
    }

    public class RepositionState : State<MushroomSniper>
    {
        public const float FarDistanceX = 50f;
        private const float RepositionSameXThreshold = 1f;
        
        public bool Finished;

        public override void Enter()
        {
            Finished = false;
        }

        public override void Update(float dt)
        {
            float deltaX = Actor.DeltaToPlayerX();
            
            if (Mathf.Abs(deltaX) <= RepositionSameXThreshold)
            {
                Finished = true;
                return;
            }

            DirectionX moveDir = deltaX > 0f ? DirectionX.Right : DirectionX.Left;
            Actor.Flip(moveDir);
            Actor.CM.GetComponent<Move>().Walk(moveDir, dt);
        }

        public override void Exit() { }
    }

    public class IdleState : State<MushroomSniper>
    {
        // Do nothing. We exit this state when we need to reposition
    }
    
    #endregion
    
    #region Shooting

    public class ShootingIdleState : TimedState<MushroomSniper>
    {
        public override void Enter()
        {
            base.Enter();
            Delay = 3f;
        }
    }

    public class ShootState : State<MushroomSniper>
    {
        private int bulletsPerBurst = 3;
        private float burstShotInterval = 0.12f;
        
        private int shotsFired;
        public bool Finished;

        public override void Enter()
        {
            shotsFired = 0;
            Finished = false;
            FireNextShot();
        }

        public override void Update(float dt) { }

        public override void Exit()
        {
            shotsFired = 0;
        }

        private void FireNextShot()
        {
            Vector2 direction = (Actor.Player.GlobalPosition - Actor.GlobalPosition).Normalized();
            
            Actor.CM.GetComponent<ProjectileSpawner>()
                .Start()
                .SetPosition(Actor.GlobalPosition)
                .SetDirectionNormal(direction)
                .AddToGame()
                .Build();

            shotsFired++;
            if (shotsFired >= bulletsPerBurst)
            {
                Finished = true;
                return;
            }

            TimerManager.Schedule(burstShotInterval, Actor, (_) => FireNextShot());
        }
    }
    
    #endregion
    
    private float DeltaToPlayerX() => Player.GlobalPosition.X - GlobalPosition.X;
    private float DistanceToPlayerX() => Mathf.Abs(DeltaToPlayerX());
    private bool IsPlayerFarAway() => DistanceToPlayerX() >= RepositionState.FarDistanceX;
    private bool IsPlayerAtOrAbove() => Player.GlobalPosition.Y <= GlobalPosition.Y;

    public override void Die()
    {
        CM.GetComponent<SaveEntity>().ChangeState(true);
        GlobalEventBus.Instance.Publish(GlobalEvents.BossDefeated);
        base.Die();
    }
}
