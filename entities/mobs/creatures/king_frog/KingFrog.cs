using Godot;
using System;
using TerraEngineer;
using TerraEngineer.entities.mobs;

public partial class KingFrog : Mob
{
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
    
    private StateMachine<Frog> fsm;
    
    private JumpState jumpState;
    private IdleState idleState;
    private SmashState smashState;
    private SpawnState spawnState;
    
    public override void _Ready()
    {
        base._Ready();
    //     fsm = new StateMachine<Frog>(this, waitState);
    //     fsm.AddTransition(jumpState, waitState, jumpState.LandedOnFloor);
    //     fsm.AddTransition(waitState, jumpState, waitState.TimerCondition);
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
    
    public class IdleState : IState<KingFrog>
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
    
    public class SpawnState : IState<KingFrog>
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
