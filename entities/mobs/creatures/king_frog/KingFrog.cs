using Godot;
using System;
using System.Collections.Generic;
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
        int halfMySize = (int)Math.Ceiling(myXSize / 2f);
        float xMaxRight = arenaBounds.XRight - halfMySize;
        float xMaxLeft = arenaBounds.XLeft + halfMySize;
        float xMiddle = (arenaBounds.XRight+arenaBounds.XLeft)/2;
        jumpState.Positions = [xMaxLeft, xMiddle, xMaxRight];
        
        fsm = new StateMachine<KingFrog>(this, idleState);
        fsm.AddTransition(idleState, jumpState, idleState.TimerCondition);
        fsm.AddTransition(jumpState, idleState, jumpState.IsFinished);
        fsm.AddTransition(jumpState, spawnState, jumpState.IsFinished);
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
        public enum ArenaPos
        {
            Left=0,
            Middle=1,
            Right=2,
        }

        public ArenaPos AmAt = ArenaPos.Middle;

        public List<float> Positions = new(); // [XMaxLeft, XMiddle, XMaxRight]

        private float goTo;
        private float speedModifier = 1f;

        private bool finished = false;
        
        private const float JumpModifierPerPixel = 0.005f;
        
        public override void Enter()
        {
            // Choose where to jump
            ArenaPos wasAt = AmAt;
            List<int> chooseFrom = [0, 1, 2];
            chooseFrom.RemoveAt((int)AmAt); // Do not choose current location
            AmAt = (ArenaPos)(int)MathT.RandomChooseList(chooseFrom);


            if ((int)AmAt > (int)wasAt)
            {
                Actor.Flip(DirectionX.Right);
            }
            else Actor.Flip(DirectionX.Left);
            
            goTo = Positions[(int)AmAt];

            float jumpModifier = 1f;
            if (Math.Abs((int)AmAt - (int)wasAt) > 1)
            {
                // big jumpino
                jumpModifier = 1.25f;
                speedModifier = 1.55f;
            }
            
            Actor.CM.GetComponent<Jump>().AttemptJump(jumpModifier);
            Actor.CM.GetComponent<Gravity>().LandedOnFloor += Finished;
        }

        public override void Update( float dt)
        {
            Actor.CM.GetComponent<Move>().WalkToPoint(goTo, speedModifier);
        }

        public override void Exit()
        {
            if(AmAt == ArenaPos.Left)
                Actor.Flip(DirectionX.Right);
            else if(AmAt == ArenaPos.Right)
                Actor.Flip(DirectionX.Left);
                
            
            Actor.CM.GetComponent<Gravity>().LandedOnFloor -= Finished;
            finished = false;
            speedModifier = 1f;
        }
        
        public bool IsFinished() => finished;
        public void Finished() { finished = true; }
    }
    
    public class IdleState : TimedState<KingFrog>
    {
        public override void Enter()
        {
            base.Enter();
            Delay = 0.5f;
        }
    }
    
    public class SpawnState : State<KingFrog>
    {
        private int amountToSpawn = 3;
        private int amountSpawned = 0;
        private float minTimeSpawn = 1f;
        private float maxTimeSpawn = 6f;
        private int frogHealth = 5;
        
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
        public void Finished() { finished = true; }
    }
    
    public class SmashState : State<KingFrog>
    {
        private bool finished = false;
        private float gravityModifier = 10f;
        
        public override void Enter()
        {
            Actor.velocity.Y = 0;
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
