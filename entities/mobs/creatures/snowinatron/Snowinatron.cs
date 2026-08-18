using Godot;
using System;
using System.Runtime.CompilerServices;
using TENamespace;
using TerraEngineer;
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
    
    public override void Init()
    {
        fsm = new (this, waitState);

        bool IsFinished() => fsm.IsTriggered(GenericCreatureTriggers.TaskFinished);
        
        fsm.AddTransition(waitState, sidesShootState, waitState.TimerCondition);
        fsm.AddTransition(waitState, shootHomingState, waitState.TimerCondition);
        fsm.AddTransition(waitState, throwSnowState, waitState.TimerCondition);
        
        fsm.AddTransition(sidesShootState, waitState, IsFinished);
        fsm.AddTransition(shootHomingState, waitState, IsFinished);
        fsm.AddTransition(throwSnowState, waitState, IsFinished);

        
        
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
        
    }
    
    public class ShootHomingState : State<Snowinatron>
    {
        
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
