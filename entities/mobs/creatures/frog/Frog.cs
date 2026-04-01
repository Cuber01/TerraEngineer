using Godot;
using System;
using TENamespace;
using TerraEngineer;
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
        private bool inAir = false;
        
        public override void Enter()
        {
            //TODO
            //Actor.Sprite.Play(Names.Animations.Jump);
            //Actor.Sprite.AnimationFinished += jump;
        }
        
        public override void Update( float dt)
        {
            if (inAir)
            {
                Actor.CM.GetComponent<Move>().Walk(Actor.Facing, dt);
                if (Actor.Velocity.Y > 0)
                {
                    //Actor.Sprite.Play(Names.Animations.Fall);
                }
            }
                
            
        }
        
        public override void Exit()
        {
            //Actor.Sprite.Play(Names.Animations.Idle);
            Actor.CM.GetComponent<Gravity>().LandedOnFloor -= landedOnFloor;
            inAir = false;
            isLandingOnFloor = false;
        }

        private void jump()
        {
            inAir = true;
            Actor.CM.GetComponent<Jump>().AttemptJump();
            //Actor.Sprite.AnimationFinished -= jump;
            Actor.CM.GetComponent<Gravity>().LandedOnFloor += landedOnFloor;
        }
    }
    
    public class WaitState : TimedState<Frog>
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
