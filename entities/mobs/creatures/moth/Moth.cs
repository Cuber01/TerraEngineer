using Godot;
using TerraEngineer;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.mobs.creatures;

public partial class Moth : FlyingCreature
{
    private readonly ChaseState chaseState = new();
    private readonly FlyingIdleState<FlyingCreature> idleState = new();
    
    public override void Init()
    {
        Player = GetNode<Player>(Names.NodePaths.Player);
        FlyAroundPoint = GlobalPosition;
        fsm = new StateMachineWithTriggers<FlyingCreature, GenericCreatureTriggers>(this, idleState);
        fsm.AddTransition(idleState, chaseState, (() => fsm.IsTriggered(GenericCreatureTriggers.EnemyDetected)));
        fsm.AddTransition(chaseState, idleState, (() => fsm.IsTriggered(GenericCreatureTriggers.EnemyLost)));
    }

    public override void _PhysicsProcess(double delta)
    {
        fsm.Update((float)delta);
        CM.UpdateComponents((float)delta);

        HandleMove();
    }

    public class ChaseState : State<FlyingCreature>
    {
        public override void Update( float dt)
        {
            Actor.CM.GetComponent<FreeFly>().FlyToPoint(Actor.Player.GlobalPosition, dt);  
        }
    }
    
    
}
