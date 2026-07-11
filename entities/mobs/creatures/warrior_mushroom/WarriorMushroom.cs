using Godot;
using Godot.Collections;
using TENamespace;
using TENamespace.basic;
using TENamespace.health;
using TerraEngineer;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.mobs.creatures;

[Tool]
public partial class WarriorMushroom : Creature
{
    [Export] private CollisionShape2D chargeHitbox;
    [Export] private Rect2 attackHitboxShape;
    
    private const float FarDistanceX = 50f;

    private StateMachine<WarriorMushroom> fsm;

    private readonly IdleState idleState = new();
    private readonly FlipState flipState = new();
    private readonly ChargeState chargeState = new();
    private readonly StuckState stuckState = new();
    private readonly JumpState jumpState = new();
    private readonly AttackState attackState = new();

    private bool swordStuckInWall;

    public Player Player { get; private set; }

    public override void Init()
    {
        Player = GetNode<Player>(Names.NodePaths.Player);

        fsm = new StateMachine<WarriorMushroom>(this, idleState);

        fsm.AddGlobalTransition(stuckState, () => swordStuckInWall);

        fsm.AddTransition(idleState, flipState, IsPlayerBehind, 2);

        fsm.AddTransition(idleState, chargeState, IsPlayerFarAway, 0, 0.5f);
        fsm.AddTransition(idleState, jumpState, IsPlayerFarAway, 0, 0.5f);
        fsm.AddTransition(idleState, attackState, IsPlayerClose, 1, 0.5f);

        fsm.AddTransition(flipState, idleState, () => flipState.Finished);

        fsm.AddTransition(chargeState, attackState, () => chargeState.Finished && IsPlayerClose());
        fsm.AddTransition(chargeState, idleState, () => chargeState.Finished && !IsPlayerClose());

        fsm.AddTransition(jumpState, attackState, () => jumpState.Finished && IsPlayerClose());
        fsm.AddTransition(jumpState, idleState, () => jumpState.Finished && !IsPlayerClose());

        fsm.AddTransition(attackState, idleState, () => attackState.Finished);
        fsm.AddTransition(stuckState, idleState, stuckState.TimerCondition);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Engine.IsEditorHint())
            return;

        fsm.Update((float)delta);
        CM.UpdateComponents((float)delta);

        HandleMove();
    }

    public class IdleState : State<WarriorMushroom>
    {
        public override void Enter()
        {
            Actor.velocity.X = 0f;
            Actor.SpriteWrapper.Play(Names.Animations.Idle);
        }

        public override void Update(float dt) { }
        public override void Exit() { }
    }

    public class FlipState : State<WarriorMushroom>
    {
        private const float BackwardMoveMultiplier = 0.55f;

        public bool Finished;

        public override void Enter()
        {
            Finished = false;
            
            DirectionX backwards = (DirectionX)(-(int)Actor.Facing);
            Actor.CM.GetComponent<Jump>().AttemptJump(0.7f);
            Actor.CM.GetComponent<Move>().Walk(backwards, 1f, BackwardMoveMultiplier);

            Actor.CM.GetComponent<Gravity>().LandedOnFloor += OnLandedOnFloor;
            Actor.Flip();
        }

        public override void Update(float dt)
        {
            DirectionX backwards = (DirectionX)(-(int)Actor.Facing);
            Actor.CM.GetComponent<Move>().Walk(backwards, dt, BackwardMoveMultiplier);
        }

        public override void Exit()
        {
            Actor.CM.GetComponent<Gravity>().LandedOnFloor -= OnLandedOnFloor;
            Finished = false;
        }

        private void OnLandedOnFloor()
        {
            Finished = true;
        }
    }

    public class ChargeState : State<WarriorMushroom>
    {
        private const int ChargeDamage = 2;
        private const float ChargeKnockback = 100f;
        private const float ChargeSpeedMultiplier = 2f;

        public bool Finished;

        public override void Enter()
        {
            Finished = false;
            Actor.SpriteWrapper.AnimationFinished += OnAnimationFinished;

            Actor.SpriteWrapper.Play("charge");
            Actor.chargeHitbox.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
        }

        public override void Update(float dt)
        {
            Actor.CM.GetComponent<Move>().Walk(Actor.Facing, dt, ChargeSpeedMultiplier);

            if (Actor.IsOnWall())
            {
                Finished = true;
            }
        }

        public override void Exit()
        {
            Actor.SpriteWrapper.AnimationFinished -= OnAnimationFinished;
            Finished = false;
            Actor.chargeHitbox.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
        }

        private void OnAnimationFinished()
        {
            Finished = true;
        }

        public void ChargeHit(Node2D body)
        {
            if (body is Entity mob)
            {
                Actor.dealAttack(mob, ChargeDamage, ChargeKnockback, Actor.GlobalPosition);
            } else if (body is TileMapLayer)
            {
                Actor.swordStuckInWall = true;
            }
        }
    }

    public class StuckState : TimedState<WarriorMushroom>
    {
        public override void Enter()
        {
            base.Enter();
            Delay = 5;
            Actor.velocity.X = 0f;
            Actor.SpriteWrapper.Play(Names.Animations.Idle);
            Actor.swordStuckInWall = false;
        }
    }

    public class JumpState : State<WarriorMushroom>
    {
        private const float JumpMoveMultiplier = 0.9f;

        public bool Finished;

        public override void Enter()
        {
            Finished = false;

            Actor.CM.GetComponent<Jump>().AttemptJump();
            Actor.CM.GetComponent<Move>().Walk(Actor.Facing, 1f, JumpMoveMultiplier);
            
            Actor.CM.GetComponent<Gravity>().LandedOnFloor += OnLandedOnFloor;
            Actor.SpriteWrapper.Play("charge");
        }

        public override void Update(float dt)
        {
            Actor.CM.GetComponent<Move>().Walk(Actor.Facing, dt, JumpMoveMultiplier);
        }

        public override void Exit()
        {
            Actor.CM.GetComponent<Gravity>().LandedOnFloor -= OnLandedOnFloor;
            Finished = false;
        }

        private void OnLandedOnFloor()
        {
            Finished = true;
        }
    }

    public class AttackState : State<WarriorMushroom>
    {
        private const int AttackCount = 3;
        private const float StepMoveTime = 0.8f;
        private const int AttackDamage = 2;
        private const float AttackKnockback = 100f;

        private int attacksDone;
        private float moveTimer;
        public bool Finished;

        public override void Enter()
        {
            attacksDone = 0;
            moveTimer = 0f;
            Finished = false;

            Actor.SpriteWrapper.AnimationFinished += OnAttackFinished;
            executeAttack();
        }

        public override void Update(float dt)
        {
            if (moveTimer > 0f)
            {
                moveTimer -= dt;
                Actor.CM.GetComponent<Move>().Walk(Actor.Facing, dt);
            }
        }

        private void OnAttackFinished()
        {
            attacksDone++;
            moveTimer = StepMoveTime;

            if (attacksDone >= AttackCount)
            {
                Finished = true;
                return;
            }
            
            executeAttack();
        }

        private void executeAttack()
        {
            Actor.SpriteWrapper.Play("attack");
            
            RectangleShape2D rect = new()
            {
                Size = Actor.attackHitboxShape.Size
            };

            Transform2D attackTransform = new Transform2D(Actor.GlobalRotation,
                new Vector2(Actor.GlobalPosition.X + (Actor.attackHitboxShape.Position.X * (int)Actor.Facing),
                            Actor.GlobalPosition.Y + Actor.attackHitboxShape.Position.Y));
            PhysicsShapeQueryParameters2D query = new()
            {
                Shape = rect,
                Transform = attackTransform,
                CollisionMask = 2 // Player
            };
         
            var spaceState = Actor.GetWorld2D().DirectSpaceState;
            Array<Dictionary> results = spaceState.IntersectShape(query, 1);
            foreach (Dictionary result in results)
            {
                Variant colliderVariant = result[Names.Properties.Collider];
                if (colliderVariant.Obj is Entity mob)
                {
                    Actor.dealAttack(mob, AttackDamage, AttackKnockback, attackTransform.Origin);
                }
            }
        }
        
        public override void Exit()
        {
            Actor.SpriteWrapper.AnimationFinished -= OnAttackFinished;

            attacksDone = 0;
            moveTimer = 0f;
            Finished = false;
        }
    }

    private void dealAttack(Entity victim, int damage, float knockbackForce, Vector2 knockbackFrom)
    {
        if(victim.GodMode) return;
            
        victim.CM.TryGetComponent<Health>()?.ChangeHealth(-damage);
                
        victim.CM.TryGetComponent<KnockbackComponent>()
            ?.ApplyKnockback(knockbackFrom, knockbackForce);
    }
    
    private void onChargeAreaEntered(Node2D body) => chargeState.ChargeHit(body);
    private float DeltaToPlayerX() => Player.GlobalPosition.X - GlobalPosition.X;
    private bool IsPlayerBehind() => Mathf.Sign(DeltaToPlayerX()) == -(int)Facing;
    private bool IsPlayerFarAway() => Mathf.Abs(DeltaToPlayerX()) >= FarDistanceX;
    private bool IsPlayerClose() => Mathf.Abs(DeltaToPlayerX()) < FarDistanceX;
}
