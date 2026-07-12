using Godot;
using System;
using System.Runtime.InteropServices.ComTypes;
using TENamespace;
using TENamespace.health;
using TENamespace.projectile_builder;
using TENamespace.save_entity;
using TerraEngineer;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.mobs.creatures;
using TerraEngineer.entities.projectiles;
using TerraEngineer.game;

// For movement fsm:
// - enter idle by default
// - if player is far away: reposition state
// - if player's y is at or above current y: flanking state
// - if neither is true: idle state

// For shooting fsm:
// - enter idle by default
// - after a timer switch to shooting
// - after finishing shooting switch back to idle

// Shooting:
// shoot three bullets with a small timer in between and make them fly at player, when all three are shot trigger a finished flag
// Idle:
// wait a timer and trigger a finished flag
// Reposition:
// move closer to player in x axis, finishes when on about the same x as player
// Flanking (second phase):
// if player is close move away from him, if player is far move closer, this state never exits 

[Tool]
public partial class SniperMushroom : Creature
{
    [Export] private CollisionShape2D hurtbox;
    
    private float hideTime = 12f;
    private bool forcedToHide = false;
    
    private StateMachine<SniperMushroom> movementFsm;
    private StateMachine<SniperMushroom> gunFsm;
    private bool publishedEntered;

    private readonly FlankingState flankingState = new();
    private readonly RepositionState repositionState = new();
    private readonly IdleState idleState = new();
    private readonly HideState hideState = new();
    
    private readonly ShootState shootState = new();
    private readonly ShootingIdleState shootingIdleState = new();
    private readonly ShootingHideState shootingHideState = new();
    
    public Player Player { get; private set; }

    public override void Init()
    {
        Player = GetNode<Player>(Names.NodePaths.Player);

        // --- MOVEMENT FSM ---
        movementFsm = new StateMachine<SniperMushroom>(this, idleState);
        movementFsm.AddTransition(idleState, repositionState, () => !IsPlayerAtOrAbove() && IsPlayerFarAway());
        movementFsm.AddTransition(repositionState, idleState, () => !IsPlayerAtOrAbove() && repositionState.Finished);
        movementFsm.AddGlobalTransition(hideState, () => forcedToHide, 1);
        movementFsm.AddTransition(hideState, idleState, hideState.TimerCondition);
        
        // If player got to us, we force the reposition state ignoring previous ones and never come back
        movementFsm.AddTransition(idleState, flankingState, IsPlayerAtOrAbove);
        movementFsm.AddTransition(repositionState, flankingState, IsPlayerAtOrAbove); 

        // --- GUN FSM ---
        gunFsm = new StateMachine<SniperMushroom>(this, shootingIdleState);
        gunFsm.AddTransition(shootingIdleState, shootState, shootingIdleState.TimerCondition);
        gunFsm.AddTransition(shootState, shootingIdleState, () => shootState.Finished);
        gunFsm.AddGlobalTransition(shootingHideState, () => forcedToHide, 1);
        gunFsm.AddTransition(shootingHideState, shootingIdleState, shootingHideState.TimerCondition);

        CM.GetComponent<Health>().HealthChanged += onDamageTaken;
    
        
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

    private void onDamageTaken(int current, int amount, Entity source)
    {
        if (source is BasicBullet && movementFsm.CurrentState is not HideState) // Bullet is mine
        {
            forcedToHide = true;
            CM.GetComponent<Health>().MakeInvincible(hideTime);   
        }
    }
    
    #region Movement
    public class FlankingState : State<SniperMushroom>
    {
        private const float OptimalDistanceX = 80f;
        private const float PositionTolerance = 1f;
        
        public override void Enter()
        {
            
        }

        public override void Update(float dt)
        {
            float deltaX = Actor.DeltaToPlayerX();
            float distX = Actor.DistanceToPlayerX();

            DirectionX moveDir = DirectionX.None;

            if(distX.IsWithin(OptimalDistanceX, PositionTolerance))
            {
                // Do nothing
            }
            else if (distX <= OptimalDistanceX)
            {
                // Move away
                moveDir = deltaX > 0f ? DirectionX.Left : DirectionX.Right;
            }
            else if (distX >= OptimalDistanceX)
            {
                // Move towards
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

    public class RepositionState : State<SniperMushroom>
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
    
    public class HideState : TimedState<SniperMushroom>
    {
        public override void Enter()
        {
            base.Enter();
            Delay = Actor.hideTime;
            Actor.SpriteWrapper.Play("hide");
            Actor.hurtbox.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
        }

        public override void Exit()
        {
            Actor.SpriteWrapper.Play("idle");
            Actor.forcedToHide = false;
            Actor.hurtbox.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
        }
    }

    public class IdleState : State<SniperMushroom>
    {
        // Do nothing. We exit this state when we need to reposition
    }
    
    #endregion
    
    #region Shooting

    public class ShootingIdleState : TimedState<SniperMushroom>
    {
        public override void Enter()
        {
            base.Enter();
            Delay = 3f;
        }
    }
    
    public class ShootingHideState : TimedState<SniperMushroom>
    {
        public override void Enter()
        {
            base.Enter();
            Delay = Actor.hideTime;
        }
    }

    public class ShootState : State<SniperMushroom>
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
