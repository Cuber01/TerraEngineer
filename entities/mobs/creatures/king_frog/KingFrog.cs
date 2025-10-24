using Godot;
using System;
using System.Collections.Generic;
using TENamespace;
using TerraEngineer;
using TerraEngineer.entities.mobs;

public partial class KingFrog : Mob
{
    [Export] private XBounds arenaBounds;
    [Export] private int myXSize;
    
    // Set arena size
    // Calculate middle, start and end based on boss size
    // Add jump to pos method that will be used for jumping between calculated points
    
    // Phases:
    // Jump => Smash or Idle
    // Idle => Spawn or Jump
    // Smash => Jump
    // Spawn => Jump
    // Stretch: tongue attack
    // If you do a secret you can get green essence early and play the boss fight with a heart plant
    // Green essence room: requires using green essence to leave, make heartplant or die to damaging blocks
    
    private StateMachine<KingFrog> fsm;
    
    private JumpState jumpState = new JumpState();
    private IdleState idleState = new IdleState();
    private SmashState smashState = new SmashState();
    private SpawnState spawnState =  new SpawnState();
    
    public override void _Ready()
    {
        base._Ready();
        
        int halfMySize = (int)Math.Ceiling(myXSize / 2f);
        float XMaxRight = arenaBounds.XRight - halfMySize;
        float XMaxLeft = arenaBounds.XLeft + halfMySize;
        float XMiddle = (arenaBounds.XRight+arenaBounds.XLeft)/2;
        jumpState.Positions = [XMaxLeft, XMiddle, XMaxRight];
        
        fsm = new StateMachine<KingFrog>(this, idleState);
        fsm.AddTransition(idleState, jumpState, idleState.TimerCondition);
        fsm.AddTransition(jumpState, idleState, jumpState.IsFinished);
        
        //fsm.AddTransition(idleState, spawnState, idleState.TimerCondition);

    }

    public override void _PhysicsProcess(double delta)
    {
        fsm.Update((float)delta);
        CM.UpdateComponents((float)delta);
        
        Velocity = velocity;
        MoveAndSlide();
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
                actor.Sprite.FlipH = false;
            }
            else actor.Sprite.FlipH = true;
            
            goTo = Positions[(int)AmAt];

            float jumpModifier = 1f;
            if (Math.Abs((int)AmAt - (int)wasAt) > 1)
            {
                // big jumpino
                jumpModifier = 1.25f;
                speedModifier = 1.55f;
            }
            
            actor.CM.GetComponent<Jump>().AttemptJump(jumpModifier);
            actor.CM.GetComponent<Gravity>().LandedOnFloor += () => finished = true;
        }

        public void Update(KingFrog actor, float dt)
        {
            actor.CM.GetComponent<Move>().WalkToPoint(goTo, speedModifier);
        }

        public void Exit(KingFrog actor)
        {
            if(AmAt == ArenaPos.Left)
                actor.Sprite.FlipH = false;
            else if(AmAt == ArenaPos.Right)
                actor.Sprite.FlipH = true;
                
            finished = false;
            speedModifier = 1f;
        }
        
        public bool IsFinished() => finished;
    }
    
    public class IdleState : TimedState<KingFrog>
    {
        public override void Enter(KingFrog actor)
        {
            base.Enter(actor);
            Delay = 0.5f;
        }
    }
    
    public class SpawnState : TimedState<KingFrog>
    {
        public override void Enter(KingFrog actor)
        {
            base.Enter(actor);
            throw new NotImplementedException();
        }

        public override void Update(KingFrog actor, float dt)
        {
            base.Update(actor, dt);
            throw new NotImplementedException();
        }

        public override void Exit(KingFrog actor)
        {
            base.Exit(actor);
            throw new NotImplementedException();
        }
    }
    
    public class SmashState : IState<KingFrog>
    {
        public void Enter(KingFrog actor)
        {
            throw new NotImplementedException();
        }

        public void Update(KingFrog actor, float dt)
        {
            throw new NotImplementedException();
        }

        public void Exit(KingFrog actor)
        {
            throw new NotImplementedException();
        }
    }
    
}
