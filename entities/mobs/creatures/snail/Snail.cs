using Godot;
using System;
using System.Security.AccessControl;
using TENamespace;
using TerraEngineer;
using TerraEngineer.entities.mobs;

public partial class Snail : Mob
{
    [Export] private RayCast2D down;
    [Export] private RayCast2D right;
    [Export] private RayCast2D left;
    [Export] public Vector2 newFacing = Vector2.Right; // Does not impact sprite since we rotate it!
    [Export] private float initTime = 1f;

    private ITimer initTimer;
    private bool init = false;
    
    private readonly WalkState walkState = new WalkState();
    private readonly RotateState rotateState = new RotateState();
    
    private StateMachine<Snail> fsm;
    public float ToRotate = 0;
    
    public override void _Ready()
    {
        fsm = new StateMachine<Snail>(this, walkState, true);
        
        fsm.AddTransition(walkState, rotateState, needsRotate);
        initTimer = TimerManager.Schedule(1f, (_) => init = true);
    }
    
    public override void _PhysicsProcess(double delta)
    {
        fsm.Update((float)delta);
        CM.UpdateComponents((float)delta);

        GD.Print(down.IsColliding());
        
        Velocity = velocity;
        MoveAndSlide();
    }

    private bool needsRotate()
    {
        if(!init) return false;
        
        if (down.IsColliding()) {
            // if (newFacing.X == 1 && right.IsColliding())
            // {
            //     ToRotate = -90;
            // }
            //
            // if (newFacing.X == -1 && left.IsColliding())
            // {
            //     ToRotate = 90;
            // }
        }
        else
        {
            ToRotate = newFacing.X * 90;
        }
        
        return (ToRotate != 0);
    }
    

    public class WalkState : IState<Snail>
    {
        public void Enter(Snail actor) { }

        public void Update(Snail actor, float dt)
        {
            //actor.CM.GetComponent<Move>().Walk4(actor.newFacing);
        }

        public void Exit(Snail actor)
        {
            actor.velocity = Vector2.Zero;
        }
    }
    
    public class RotateState : IState<Snail>
    {
        private float rotationSpeed = 1f;
        private float reachRotation = 0;
        private const float rotationTolerance = 1f;

        public void Enter(Snail actor)
        {
            reachRotation = actor.ToRotate + actor.RotationDegrees;
            actor.newFacing = MathT.rotateVec2(actor.newFacing, actor.ToRotate > 0);
            actor.UpDirection = -actor.newFacing;
            
            actor.ToRotate = 0;
            
        }

        public void Update(Snail actor, float dt)
        {
           // actor.CM.GetComponent<Move>().Walk4(actor.newFacing);
            actor.RotationDegrees = MathT.Lerp(actor.RotationDegrees, reachRotation, rotationSpeed);

            if (Math.Abs(actor.RotationDegrees - reachRotation) < rotationTolerance)
                actor.fsm.ChangeState(actor.walkState);
        }

        public void Exit(Snail actor)
        {
            reachRotation = 0;
        }
    }
    

}
