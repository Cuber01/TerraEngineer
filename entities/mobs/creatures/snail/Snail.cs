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
    [Export] public new Direction4 Facing = Direction4.Right; // Does not impact sprite since we rotate it!
    
    private readonly WalkState walkState = new WalkState();
    private readonly RotateState rotateState = new RotateState();
    
    private StateMachine<Snail> fsm;
    public int ToRotate = 0;
    
    public override void _Ready()
    {
        fsm = new StateMachine<Snail>(this, walkState, true);
        
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
        if (down.IsColliding()) {
            if (Facing == Direction4.Right && right.IsColliding())
            {
                ToRotate = -90;
            }
            
            if (Facing == Direction4.Left && left.IsColliding())
            {
                ToRotate = 90;
            }
        }
        else
        {
            ToRotate = (int)Facing * 90;
        }
        
        return (ToRotate != 0);
    }
    

    public class WalkState : IState<Snail>
    {
        public void Enter(Snail actor) { }

        public void Update(Snail actor, float dt)
        {
            actor.CM.GetComponent<Move>().Walk4(MathT.dir4ToVect2(actor.Facing));
        }

        public void Exit(Snail actor) { }

    }
    
    public class RotateState : IState<Snail>
    {
        private float rotationSpeed = 10f;
        private float reachRotation = 0;
        private const float rotationTolerance = 1f;

        public void Enter(Snail actor)
        {
            reachRotation = actor.ToRotate + actor.RotationDegrees;
            actor.Facing = MathT.rotateDir4(actor.Facing, actor.ToRotate > 0);
            
            actor.ToRotate = 0;
            
        }

        public void Update(Snail actor, float dt)
        {
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
