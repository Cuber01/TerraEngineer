using Godot;
using System;
using TENamespace;
using TENamespace.basic;
using TerraEngineer;
using TerraEngineer.entities.projectiles;

public partial class Snowdrone : Projectile
{
    private readonly PretendState pretendState = new PretendState();
    private readonly ChaseState chaseState = new ChaseState();

    private const float distanceAtWhichSpriteStartsChanging = 100f;
    private const float chaseDist = 20f;

    private StateMachine<Snowdrone> fsm;

    private Player player;
    private float dist;
    
    public override void _Ready()
    {
        base._Ready();
        player = GetNode<Player>(Names.NodePaths.Player);
        InitSpriteWrapper();

        fsm = new StateMachine<Snowdrone>(this, pretendState);
        fsm.AddTransition(pretendState, chaseState, () => dist < chaseDist);
    }
    
    public override void _PhysicsProcess(double delta)
    {
        #if TOOLS
        if (Engine.IsEditorHint())
            return;
        #endif
        
        fsm.Update((float)delta);
        CM.UpdateComponents((float)delta);
        
        HandleMove();
        FlipIfHitWall();
    }

    private void updateFrame()
    {
        dist = GlobalPosition.DistanceTo(player.GlobalPosition);
        int maxFrame = SpriteWrapper.GetFrameCount(Names.Animations.Default);
        float step = distanceAtWhichSpriteStartsChanging / maxFrame;
        int stepAmount = Mathf.RoundToInt(dist / step);
        SpriteWrapper.SetFrame(maxFrame-stepAmount);
    }

    public class PretendState : State<Snowdrone>
    {
        
        
        public override void Update( float dt)
        {
            Actor.updateFrame();
            
            TrigFly fly = Actor.CM.GetComponent<TrigFly>();
            fly.FlyToPoint(Actor.player.GlobalPosition, dt, fly.FlyInDirectionSinusoidal);
        }
    }
    
    public class ChaseState : State<Snowdrone>
    {
        public override void Enter()
        {
            Actor.CM.GetComponent<TrigFly>().MultiplyAcceleration(2);
        }

        public override void Update(float dt)
        {
            Actor.updateFrame();
            
            Actor.CM.GetComponent<TrigFly>().FlyToPoint(Actor.player.GlobalPosition, dt);
        }

        public override void Exit()
        {
            Actor.CM.GetComponent<TrigFly>().MultiplyAcceleration(0.5f);
        }
    }

}
