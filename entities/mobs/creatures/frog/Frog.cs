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
        // Society if C# had real defines...
        #if TOOLS
        if (Engine.IsEditorHint())
            return;
        #endif

        
        fsm.Update((float)delta);
        CM.UpdateComponents((float)delta);
        
        Velocity = velocity;
        MoveAndSlide();

        for(int i = 0; i < GetSlideCollisionCount(); i++)
        {
            Vector2 normal = GetSlideCollision(i).GetNormal();
            if (normal == new Vector2(-(int)Facing, 0))
            {
                Flip();
            }
        }
    }

    public class JumpState : IState<Frog>
    {
        public Func<bool> LandedOnFloor => () => isLandingOnFloor;
        private bool isLandingOnFloor = false;
        private void landedOnFloor() => isLandingOnFloor = true;
        
        public void Enter(Frog actor)
        {
            isLandingOnFloor = false;
            actor.CM.GetComponent<Jump>().AttemptJump();
            actor.CM.GetComponent<Gravity>().LandedOnFloor += landedOnFloor;
        }
        
        public void Update(Frog actor, float dt)
        {
            actor.CM.GetComponent<Move>().Walk(actor.Facing, dt);
        }
        
        public void Exit(Frog actor)
        {
            actor.CM.GetComponent<Gravity>().LandedOnFloor -= landedOnFloor;
        }
    }
    
    public class WaitState : TimedState<Frog>
    {
        public override void Enter(Frog actor)
        {
            base.Enter(actor);
            Delay = 2.5f;
        }

        public override void Update(Frog actor, float dt)
        {
            base.Update(actor, dt);
        }
    }

}
