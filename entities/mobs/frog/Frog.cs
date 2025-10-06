using Godot;
using System;
using TENamespace;
using TENamespace.health;
using TerraEngineer;
using TerraEngineer.entities.mobs;

public partial class Frog : Mob
{
    private readonly JumpState jumpState = new JumpState();
    private readonly TimedState<Frog> waitState = new WaitState();
    
    private StateMachine<Frog> fsm;
    
    public override void _Ready()
    {
        fsm = new StateMachine<Frog>(this, waitState);
        fsm.AddTransition(jumpState, waitState, jumpState.LandedOnFloor);
        fsm.AddTransition(waitState, jumpState, waitState.TimerCondition);
    }

    public override void _PhysicsProcess(double delta)
    {
        fsm.Update((float)delta);

        Velocity = velocity;
        MoveAndSlide();

        for(int i = 0; i < GetSlideCollisionCount(); i++)
        {
            Vector2 normal = GetSlideCollision(i).GetNormal();
            GD.Print(normal);
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
            actor.CM.GetComponent<Move>().Walk(actor.Facing);
            actor.CM.GetComponent<Move>().UpdateFriction();
            actor.CM.GetComponent<Gravity>().UpdateGravity(dt);
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
            actor.CM.GetComponent<Gravity>().UpdateGravity(dt);
            actor.CM.GetComponent<Move>().UpdateFriction();
        }
    }

}
