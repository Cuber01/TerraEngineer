using Godot;
using System;
using System.Security.AccessControl;
using TENamespace;
using TerraEngineer;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.mobs.creatures;

[Tool]
public partial class Snail : Creature
{
    [Export] private RayCast2D down;
    [Export] private RayCast2D right;
    [Export] private RayCast2D left;
    [Export] public Vector2I vecFacing = Vector2I.Right; // Does not impact sprite since we rotate it!
    [Export] private float initTime = 1f;
    
    private readonly WalkState walkState = new WalkState();
    private readonly RotateState rotateState = new RotateState();

    private Vector2 startingFacing;
    
    private bool raycastReady = false;
    public bool WasOnFloor = true;
    
    private StateMachine<Snail> fsm;
    public float ToRotate = 0;
    
    public override void Init()
    {
        fsm = new StateMachine<Snail>(this, walkState, true);

        CM.GetComponent<Gravity>().LandedOnFloor += () => raycastReady = true;
        
        startingFacing = vecFacing;
        
        fsm.AddTransition(walkState, rotateState, needsRotate);
    }
    
    public override void _PhysicsProcess(double delta)
    {
        #if TOOLS
        if(Engine.IsEditorHint())
            return;
        #endif
        
        fsm.Update((float)delta);
        CM.UpdateComponents((float)delta);
        
        HandleMove();
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
    

    public class WalkState : State<Snail>
    {
        public override void Enter() { }

        public override void Update(float dt)
        {
            Actor.CM.GetComponent<Move>().Walk4(Actor.vecFacing, dt); 
            // TODO Doesn't this double as friction if direction = 0?
        }

        public override void Exit()
        {
            Actor.velocity = Vector2.Zero;
        }
    }
    
    public class RotateState : State<Snail>
    {
        private float rotationDelay = 1.0f;
        private float rotationSpeed = 0.1f;
        private float reachRotation = 0;
        private const float rotationTolerance = 0.001f;

        public override void Enter()
        {

            reachRotation = Actor.ToRotate + Actor.Rotation;
            Actor.vecFacing = (Vector2I)MathT.rotateVec2(Actor.vecFacing, Actor.ToRotate > 0);
            Actor.UpDirection = MathT.rotateVec2((Vector2I)Actor.UpDirection, Actor.ToRotate > 0);
            Actor.ToRotate = 0;
            Actor.WasOnFloor = false;
            TimerManager.Schedule(rotationDelay, Actor, (_) => Actor.WasOnFloor = true);
        }

        public override void Update(float dt)
        {
            
            Actor.Rotation = Mathf.RotateToward(Actor.Rotation, reachRotation, rotationSpeed);
            
            Actor.CM.GetComponent<Move>().Walk4(Actor.vecFacing, dt);
            
            if (Math.Abs(Actor.Rotation - reachRotation) < rotationTolerance)
                Actor.fsm.ChangeState(Actor.walkState);
        }

        public override void Exit()
        {
            Actor.velocity = Vector2.Zero; // Disable effects of past gravity when changing rotation
            reachRotation = 0;
        }
    }

}
