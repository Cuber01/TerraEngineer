using Godot;
using System;
using TENamespace;
using TENamespace.health;
using TerraEngineer;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.mobs.creatures;

[Tool]
public partial class Frog : Creature
{
    private readonly JumpState jumpState = new JumpState();
    private readonly TimedState<Frog> waitState = new WaitState();

    private StateMachine<Frog> fsm;
    
    public override void Init()
    {
        fsm = new StateMachine<Frog>(this, waitState);
        fsm.AddTransition(jumpState, waitState, jumpState.LandedOnFloor);
        fsm.AddTransition(waitState, jumpState, waitState.TimerCondition);
    }
    
    public override void _PhysicsProcess(double delta)
    {
        // Society if C# had real macros...
        #if TOOLS
        if (Engine.IsEditorHint())
            return;
        #endif

        
        fsm.Update((float)delta);
        CM.UpdateComponents((float)delta);
        
        HandleMove();
        FlipIfHitWall();
    }

    public class JumpState : State<Frog>
    {
        public Func<bool> LandedOnFloor => () => isLandingOnFloor;
        private bool isLandingOnFloor = false;
        private void landedOnFloor() => isLandingOnFloor = true;
        
        public override void Enter()
        {
            isLandingOnFloor = false;
            Actor.CM.GetComponent<Jump>().AttemptJump();
            Actor.CM.GetComponent<Gravity>().LandedOnFloor += landedOnFloor;
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
    
    public class WaitState : TimedState<Frog>
    {
        public override void Enter()
        {
            base.Enter();
            Delay = 2.5f;
        }

        public override void Update(float dt)
        {
            base.Update(dt);
        }
    }

}
