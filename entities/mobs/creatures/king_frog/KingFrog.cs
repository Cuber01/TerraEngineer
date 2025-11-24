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
        
        Velocity = velocity;
        MoveAndSlide();
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

    public class JumpState : IState<KingFrog>
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
        
        public void Enter(KingFrog actor)
        {
            // Choose where to jump
            ArenaPos wasAt = AmAt;
            List<int> chooseFrom = [0, 1, 2];
            chooseFrom.RemoveAt((int)AmAt); // Do not choose current location
            AmAt = (ArenaPos)(int)MathT.RandomChooseList(chooseFrom);


            if ((int)AmAt > (int)wasAt)
            {
                actor.Flip(DirectionX.Right);
            }
            else actor.Flip(DirectionX.Left);
            
            goTo = Positions[(int)AmAt];

            float jumpModifier = 1f;
            if (Math.Abs((int)AmAt - (int)wasAt) > 1)
            {
                // big jumpino
                jumpModifier = 1.25f;
                speedModifier = 1.55f;
            }
            
            actor.CM.GetComponent<Jump>().AttemptJump(jumpModifier);
            actor.CM.GetComponent<Gravity>().LandedOnFloor += Finished;
        }

        public void Update(KingFrog actor, float dt)
        {
            actor.CM.GetComponent<Move>().WalkToPoint(goTo, speedModifier);
        }

        public void Exit(KingFrog actor)
        {
            if(AmAt == ArenaPos.Left)
                actor.Flip(DirectionX.Right);
            else if(AmAt == ArenaPos.Right)
                actor.Flip(DirectionX.Left);
                
            
            actor.CM.GetComponent<Gravity>().LandedOnFloor -= Finished;
            finished = false;
            speedModifier = 1f;
        }
        
        public bool IsFinished() => finished;
        public void Finished() { finished = true; }
    }
    
    public class IdleState : TimedState<KingFrog>
    {
        public override void Enter(KingFrog actor)
        {
            base.Enter(actor);
            Delay = 0.5f;
        }
    }
    
    public class SpawnState : IState<KingFrog>
    {
        private int amountToSpawn = 3;
        private int amountSpawned = 0;
        private float minTimeSpawn = 1f;
        private float maxTimeSpawn = 6f;
        private int frogHealth = 5;
        
        private bool finished = false;
        
        public void Enter(KingFrog actor)
        {
           spawn(actor);
        }

        public void Update(KingFrog actor, float dt) { }

        public void Exit(KingFrog actor)
        {
            finished = false;
            amountSpawned = 0;
        }

        private void spawn(KingFrog actor)
        {
            if(actor.Dead) return;
            
            actor.CM.GetComponent<CreatureSpawner>()
                .Start()
                .SetPosition(actor.CM.GetComponent<CreatureSpawner>().GlobalPosition)
                .AddToGame()
                .SetFacing(actor.Facing)
                .SetHealth(frogHealth)
                .Build();
                
            amountSpawned++;

            if (amountSpawned < amountToSpawn)
            {
                TimerManager.Schedule(MathT.RandomFloat(minTimeSpawn, maxTimeSpawn), 
                    actor,        
                    (_) => spawn(actor));
            }
            else
            {
                finished = true;
            }
        }
        
        public bool IsFinished() => finished;
        public void Finished() { finished = true; }
    }
    
    public class SmashState : IState<KingFrog>
    {
        private bool finished = false;
        private float gravityModifier = 10f;
        
        public void Enter(KingFrog actor)
        {
            actor.velocity.Y = 0;
            actor.CM.GetComponent<Gravity>().GravityForce *= gravityModifier;
            actor.CM.GetComponent<Gravity>().LandedOnFloor += Finished;
        }

        public void Update(KingFrog actor, float dt) { }

        public void Exit(KingFrog actor)
        {
            actor.CM.GetComponent<Gravity>().GravityForce /= gravityModifier;
            actor.CM.GetComponent<Gravity>().LandedOnFloor -= Finished;
            finished = false;
        }
        
        public bool IsFinished() => finished;
        public void Finished() { finished = true; }
    }
    
    private bool canSmash() => (playerDetector.IsColliding() && velocity.Y > 0);
}
