using Godot;
using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using TENamespace;
using TENamespace.basic.builders.creature_builder;
using TerraEngineer;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.mobs.creatures;

[Tool]
public partial class KingFrog : Creature
{
    [Export] private XBounds arenaBounds;
    [Export] private int myXSize;
    [Export] private RayCast2D playerDetector;
    
    // Phases:
    // Jump => Smash or Idle
    // Idle => Spawn or Jump
    // Smash => Jump
    // Spawn => Jump
    // Stretch: tongue attack
    // If you do a secret you can get green essence early and play the boss fight with a heart plant
    // Green essence room: requires using green essence to leave, make heartplant or die to damaging blocks
    // TODO: Make it so that he can smash only if his velocity is already < 0
    
    private StateMachine<KingFrog> fsm;
    
    private JumpState jumpState = new JumpState();
    private IdleState idleState = new IdleState();
    private SmashState smashState = new SmashState();
    private SpawnState spawnState =  new SpawnState();
    
    public override void Init()
    {
        // int halfMySize = (int)Math.Ceiling(myXSize / 2f);
        // float xMaxRight = arenaBounds.XRight - halfMySize;
        // float xMaxLeft = arenaBounds.XLeft + halfMySize;
        // float xMiddle = (arenaBounds.XRight+arenaBounds.XLeft)/2;
        // jumpState.Positions = [xMaxLeft, xMiddle, xMaxRight];
        
        fsm = new StateMachine<KingFrog>(this, idleState);
        fsm.AddTransition(idleState, jumpState, idleState.Condition);
        fsm.AddTransition(jumpState, idleState, jumpState.IsFinished, 0.7f);
        fsm.AddTransition(jumpState, spawnState, jumpState.IsFinished, 0.3f);
        fsm.AddTransition(jumpState, smashState, canSmash);
        fsm.AddTransition(smashState, idleState, smashState.IsFinished);
        fsm.AddTransition(spawnState, idleState, spawnState.IsFinished);
    }

    public override void _PhysicsProcess(double delta)
    {
        if(Engine.IsEditorHint())
            return;
        
        fsm.Update((float)delta);
        CM.UpdateComponents((float)delta);
        
        HandleMove();
        FlipIfHitWall();
    }
    
    protected override void FlipEffect()
    {
        base.FlipEffect();

        var spawner = CM.TryGetComponent<CreatureSpawner>();
        if (spawner != null)
        {
            spawner.Position = -spawner.Position;   
        }
    }

    public class JumpState : State<KingFrog>
    {
        public Func<bool> LandedOnFloor => () => isLandingOnFloor;
        private bool isLandingOnFloor = false;
        private void landedOnFloor() => isLandingOnFloor = true;
        public bool IsFinished() => isLandingOnFloor;
        
        public override void Enter()
        {
            Actor.CM.GetComponent<Gravity>().LandedOnFloor += landedOnFloor;
            isLandingOnFloor = false;
            
            Actor.CM.GetComponent<Jump>().AttemptJump( MathT.RandomInt(0,1)==1 ? 1.2f : 1f);
        }
        
        public override void Update( float dt)
        {
            Actor.CM.GetComponent<Move>().Walk(Actor.Facing, dt);
        }
        
        public override void Exit()
        {
            Actor.CM.GetComponent<Gravity>().LandedOnFloor -= landedOnFloor;
        }
    }
    
    public class IdleState : State<KingFrog>
    {
        private float time = 0;
        private float delay = 0.5f;

        public override void Enter()
        {
            time = 0;
        }

        public override void Update(float dt) {
            if (Actor.IsOnFloor())
            {
                time += dt;    
            }
        }
    
        public bool Condition()
        {
            return (time >= delay && Actor.IsOnFloor());
        }
    }
    
    public class SpawnState : State<KingFrog>
    {
        private int amountToSpawn = 3;
        private int amountSpawned = 0;
        private float minTimeSpawn = 1f;
        private float maxTimeSpawn = 2f;
        private int frogHealth = 2;
        
        private bool finished = false;
        
        public override void Enter()
        {
           spawn(Actor);
        }

        public override void Update( float dt) { }

        public override void Exit()
        {
            finished = false;
            amountSpawned = 0;
        }

        private void spawn(KingFrog Actor)
        {
            if(Actor.Dead) return;
            
            Actor.CM.GetComponent<CreatureSpawner>()
                .Start()
                .SetPosition(Actor.CM.GetComponent<CreatureSpawner>().GlobalPosition)
                .AddToGame()
                .SetFacing(Actor.Facing)
                .SetHealth(frogHealth)
                .Build();
                
            amountSpawned++;

            if (amountSpawned < amountToSpawn)
            {
                TimerManager.Schedule(MathT.RandomFloat(minTimeSpawn, maxTimeSpawn), 
                    Actor,        
                    (_) => spawn(Actor));
            }
            else
            {
                finished = true;
            }
        }
        
        public bool IsFinished() => finished;
    }
    
    public class SmashState : State<KingFrog>
    {
        private bool finished = false;
        private float gravityModifier = 10f;
        
        public override void Enter()
        {
            Actor.velocity.Y = 0;
            Actor.velocity.X = 0;
            Actor.CM.GetComponent<Gravity>().GravityForce *= gravityModifier;
            Actor.CM.GetComponent<Gravity>().LandedOnFloor += Finished;
        }

        public override void Update( float dt) { }

        public override void Exit()
        {
            Actor.CM.GetComponent<Gravity>().GravityForce /= gravityModifier;
            Actor.CM.GetComponent<Gravity>().LandedOnFloor -= Finished;
            finished = false;
        }
        
        public bool IsFinished() => finished;
        public void Finished() { finished = true; }
    }
    
    private bool canSmash() => (playerDetector.IsColliding() && velocity.Y > 0);
}
