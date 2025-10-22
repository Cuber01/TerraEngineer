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
    [Export] public Vector2 vecFacing = Vector2.Right; // Does not impact sprite since we rotate it!
    [Export] private float initTime = 1f;

    private ITimer initTimer;
    
    private readonly WalkState walkState = new WalkState();
    private readonly RotateState rotateState = new RotateState();

    private Vector2 startingFacing;
    
    private bool raycastReady = false;
    public bool WasOnFloor = true;
    
    private StateMachine<Snail> fsm;
    public float ToRotate = 0;
    
    public override void _Ready()
    {
        fsm = new StateMachine<Snail>(this, walkState, true);

        CM.GetComponent<Gravity>().LandedOnFloor += () => raycastReady = true;
        
        startingFacing = vecFacing;
        
        fsm.AddTransition(walkState, rotateState, needsRotate);
    }
    
    public override void _PhysicsProcess(double delta)
    {
        fsm.Update((float)delta);
        CM.UpdateComponents((float)delta);
        
        Velocity = velocity;
        MoveAndSlide();
    }

    private bool needsRotate()
    {
        // To avoid weird raycast edge cases while rotating and on instantiation 
        if(!WasOnFloor || !raycastReady) return false;
        
        if (down.IsColliding()) {
            if (vecFacing.X == 1 && right.IsColliding())
            {
                ToRotate = -MathT.PI/2;
            }
            
            if (vecFacing.X == -1 && left.IsColliding())
            {
                ToRotate = (MathT.PI/2);
            }
        }
        else
        {
            ToRotate = startingFacing.X * (MathT.PI/2);
        }
        
        return (ToRotate != 0);
    }
    

    public class WalkState : IState<Snail>
    {
        public void Enter(Snail actor) { }

        public void Update(Snail actor, float dt)
        {
            actor.CM.GetComponent<Move>().Walk4(actor.vecFacing); 
            // TODO Doesn't this double as friction if direction = 0?
        }

        public void Exit(Snail actor)
        {
            actor.velocity = Vector2.Zero;
        }
    }
    
    public class RotateState : IState<Snail>
    {
        private float rotationDelay = 1.5f;
        private float rotationSpeed = 0.1f;
        private float reachRotation = 0;
        private const float rotationTolerance = 0.001f;

        public void Enter(Snail actor)
        {
            reachRotation = actor.ToRotate + actor.Rotation;
            actor.vecFacing = MathT.rotateVec2(actor.vecFacing, actor.ToRotate > 0);
            actor.UpDirection = MathT.rotateVec2(actor.UpDirection, actor.ToRotate > 0);
            actor.ToRotate = 0;
            actor.WasOnFloor = false;
            TimerManager.Schedule(rotationDelay, (_) => actor.WasOnFloor = true);
        }

        public void Update(Snail actor, float dt)
        {
            
            actor.Rotation = Mathf.RotateToward(actor.Rotation, reachRotation, rotationSpeed);
            GD.Print(reachRotation);
            
            actor.CM.GetComponent<Move>().Walk4(actor.vecFacing);
            
            if (Math.Abs(actor.Rotation - reachRotation) < rotationTolerance)
                actor.fsm.ChangeState(actor.walkState);
        }

        public void Exit(Snail actor)
        {
            reachRotation = 0;
        }
    }

}
