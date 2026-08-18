using Godot;
using System;
using System.Runtime.CompilerServices;
using TENamespace;
using TENamespace.projectile_builder;
using TerraEngineer;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.mobs.creatures;

public partial class Snowinatron : Creature
{
	[ExportGroup("Internal")]

	[Export] private PackedScene basicBulletScene;
	[Export] private PackedScene homingBulletScene;

	[ExportGroup("External")]
	[Export] private ReferenceRect arena;
	
    private readonly SidesShootState sidesShootState = new SidesShootState();
    private readonly ShootHomingState shootHomingState = new ShootHomingState();
    private readonly ThrowSnowState throwSnowState = new ThrowSnowState();
    private readonly RecoverState recoverState = new RecoverState();
    private readonly TimedState<Snowinatron> waitState = new WaitState();

    private StateMachineWithTriggers<Snowinatron, GenericCreatureTriggers> fsm;
    private Player player;
    
    public override void Init()
    {
        player = GetNode<Player>(Names.NodePaths.Player);
        
        fsm = new (this, waitState);

        bool IsFinished() => fsm.IsTriggered(GenericCreatureTriggers.TaskFinished);
        
        fsm.AddTransition(waitState, sidesShootState, waitState.TimerCondition);
        // fsm.AddTransition(waitState, shootHomingState, waitState.TimerCondition);
        // fsm.AddTransition(waitState, throwSnowState, waitState.TimerCondition);
        
        fsm.AddTransition(sidesShootState, waitState, ()=>true);
        // fsm.AddTransition(shootHomingState, waitState, IsFinished);
        // fsm.AddTransition(throwSnowState, waitState, IsFinished);
        
        // Actor.fsm.FireTrigger(GenericCreatureTriggers.TaskFinished);
    }
    
    public override void _PhysicsProcess(double delta)
    {
        fsm.Update((float)delta);
        CM.UpdateComponents((float)delta);
        
        HandleMove();
        FlipIfHitWall();
    }

    public class SidesShootState : State<Snowinatron>
    {
        private const int DistFromArenaBounds = 8;
        private const int DistBetweenBullets = 8;

        // We do some hoops here to make sure that the player doesn't get shot in the face
        public override void Enter()
        {
            (bool isLeft, bool isUp) = whereIsPlayer();

            void ChooseLeftRight()
            {
                if(isLeft)
                    shoot(Direction4.Right);
                else
                    shoot(Direction4.Left);
            }
            
            if (isUp) {
                ChooseLeftRight();
            }
            else
            {
                bool shootUp = MathT.RandomBool();
                if (shootUp) {
                    shoot(Direction4.Up);
                }
                else {
                    ChooseLeftRight();
                }
            }
            
            Actor.fsm.FireTrigger(GenericCreatureTriggers.TaskFinished);
        }

        private void shoot(Direction4 direction)
        {
            Vector2 arenaSize = Actor.arena.Size;
            Vector2 arenaPos = Actor.arena.GlobalPosition;
            
            switch (direction)
            {
                case Direction4.Left:
                {
                    Vector2 startPos = arenaPos + DistFromArenaBounds*Vector2.One;
                    float boundDown = arenaPos.Y + arenaSize.Y - DistFromArenaBounds;
                    int bulletAmount = (int)((boundDown - startPos.Y) / DistBetweenBullets);
                    int missingBullet = MathT.RandomInt(1, bulletAmount);

                    int bulletId = 0;
                    for (float y = startPos.Y; y < boundDown; y += DistBetweenBullets)
                    {
                        // There's a randomly chosen "entrance" through the bullet wall
                        if (bulletId != missingBullet || bulletId != missingBullet - 1)
                        {
                            spawnBullet(new Vector2(startPos.X, y), Vector2.Right);    
                        }
                        bulletId += 1;
                    }
                }
                    break;
                case Direction4.Right:
                {
                    Vector2 startPos = new Vector2(arenaPos.X+arenaSize.X-DistFromArenaBounds, arenaPos.Y+DistFromArenaBounds);
                    float boundDown = arenaPos.Y + arenaSize.Y - DistFromArenaBounds;
                    int bulletAmount = (int)((boundDown - startPos.Y) / DistBetweenBullets);
                    int missingBullet = MathT.RandomInt(1, bulletAmount);
                    
                    int bulletId = 0;
                    for (float y = startPos.Y; y < boundDown; y += DistBetweenBullets)
                    {
                        // There's a randomly chosen "entrance" through the bullet wall
                        if (bulletId != missingBullet || bulletId != missingBullet - 1)
                        {
                            spawnBullet(new Vector2(startPos.X, y), Vector2.Left);
                        }
                        bulletId += 1;
                    }
                }
                    break;
                case Direction4.Up:
                {
                    Vector2 startPos = arenaPos + DistFromArenaBounds*Vector2.One;
                    float boundRight = arenaPos.X + arenaSize.X - DistFromArenaBounds;
                    int bulletAmount = (int)((boundRight - startPos.X) / DistBetweenBullets);
                    int missingBullet = MathT.RandomInt(1, bulletAmount);
                    
                    int bulletId = 0;
                    for (float x = startPos.X; x < arenaPos.X + arenaSize.X - DistFromArenaBounds; x += DistBetweenBullets)
                    {
                        if (bulletId != missingBullet || bulletId != missingBullet - 1)
                        {
                            spawnBullet(new Vector2(x, startPos.Y), Vector2.Down);
                        }
                        bulletId += 1;
                    }
                }
                    break;
                default:
                    throw new Exception("Invalid direction");
            }
        }

        private void spawnBullet(Vector2 pos, Vector2 direction)
        {
            Actor.CM.GetComponent<ProjectileSpawner>()
                .Start(Actor.basicBulletScene)
                .SetPosition(pos)
                .SetDirectionNormal(direction)
                .Build();
            
            Actor.CM.GetComponent<ProjectileSpawner>().AddToGame();
        }

        private (bool, bool) whereIsPlayer()
        {
            Vector2 arenaSize = Actor.arena.Size;
            Vector2 playerPosition = Actor.player.GlobalPosition;
            bool isLeft = false;
            bool isUp = false;
            
            if (playerPosition.X < Actor.arena.GlobalPosition.X + (arenaSize.X / 2) )
                isLeft = true;
            if (playerPosition.Y < Actor.arena.GlobalPosition.Y + (arenaSize.Y / 2) )
                isUp = true;

            return (isLeft, isUp);
        }
    }
    
    public class ShootHomingState : State<Snowinatron>
    {
        private const float TimeBetweenBullets = 1f;
        private const int MaxBullets = 4;
        private const float ChanceOfNextBullet = 0.8f;
        private int bulletsShot = 0;
        
        public void Enter()
        {
            bulletsShot = 0;
            spawnBullet(null);
        }
        
        private void spawnBullet(ITimer _)
        {
            Actor.CM.GetComponent<ProjectileSpawner>()
                .Start(Actor.homingBulletScene)
                .SetPosition(Actor.GlobalPosition)
                .Build();
            
            Actor.CM.GetComponent<ProjectileSpawner>().AddToGame();
            
            bulletsShot += 1;
            considerReschedule();
        }

        private void considerReschedule()
        {
            if (bulletsShot >= MaxBullets || MathT.RandomBool(1f-ChanceOfNextBullet))
            {
                Actor.fsm.FireTrigger(GenericCreatureTriggers.TaskFinished);
                return;
            }
            TimerManager.Schedule(TimeBetweenBullets, Actor, spawnBullet);
        }
    }
    
    public class RecoverState : State<Snowinatron>
    {
        
    }

    public class ThrowSnowState : State<Snowinatron>
    {
        
    }
    
    public class WaitState : TimedState<Snowinatron>
    {
        
        public override void Enter()
        {
            base.Enter();
            Delay = 2.0f;
        }

        public override void Update(float dt)
        {
            base.Update(dt);
        }
    }
}
