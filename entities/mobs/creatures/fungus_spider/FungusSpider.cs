using Godot;
using System;
using TENamespace;
using TerraEngineer;
using TerraEngineer.entities.mobs.creatures;


[Tool]
public partial class FungusSpider : Creature
{
    [Export] private RayCast2D isGroundAhead;
    
    private readonly WalkState walkState = new WalkState();

    private StateMachine<FungusSpider> fsm;
    
    public override void Init()
    {
        fsm = new StateMachine<FungusSpider>(this, walkState);
    }
    
    public override void _PhysicsProcess(double delta)
    {
        // Society if C# had real macros...
        #if TOOLS
        if (Engine.IsEditorHint())
            return;
        #endif

        if (!isGroundAhead.IsColliding())
        {
            Flip();
        }
        
        fsm.Update((float)delta);
        CM.UpdateComponents((float)delta);
        
        FlipIfHitWall();
        
        HandleMove();
    }

    public class WalkState : State<FungusSpider>
    {
        public override void Enter() { }
        
        public override void Update(float dt)
        {
            Actor.CM.GetComponent<Move>().Walk(Actor.Facing, dt);
        }
        
    }
    
    protected override void FlipEffect()
    {
        base.FlipEffect();
        isGroundAhead.Position = new Vector2(-isGroundAhead.Position.X, isGroundAhead.Position.Y);
    }
    
    
}
