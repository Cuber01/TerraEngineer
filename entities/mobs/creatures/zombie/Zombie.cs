using Godot;
using System;
using TENamespace;
using TerraEngineer;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.mobs.creatures;

[Tool]
public partial class Zombie : Creature
{
	[Export] public RayCast2D IsWallAhead;
	[Export] private RayCast2D isEnemyAbove;
	[Export] private PackedScene zombieBodyScene;
	[Export] private float alertedSpeed = 80;
    
	private readonly WalkState walkState = new WalkState();
	private readonly JumpState jumpState = new JumpState();

	private StateMachine<Zombie> fsm;
	private Player player;
	public bool Alerted = false;
	
	public override void Init()
	{
		player = GetNode<Player>(Names.NodePaths.Player);
		fsm = new StateMachine<Zombie>(this, walkState);
		fsm.AddTransition(walkState, jumpState, () =>
		{
			if (Alerted)
			{
				return (IsWallAhead.IsColliding() || isEnemyAbove.IsColliding());	
			}
			else
			{
				return false;
			}
		});
		fsm.AddTransition(jumpState, walkState, jumpState.LandedOnFloor);
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
	}

	public class WalkState : State<Zombie>
	{
		public override void Enter() { }
        
		public override void Update(float dt)
		{
			if (Actor.Alerted)
			{
				alertedUpdate();
			}
			else
			{
				unalertedUpdate();
			}
			
			Actor.CM.GetComponent<Move>().Walk(Actor.Facing, dt);
		}

		private void alertedUpdate()
		{
			float playerSide = Actor.player.Position.X - Actor.Position.X;
			if (playerSide < 0 && Actor.Facing == DirectionX.Right || 
			    playerSide > 0 && Actor.Facing == DirectionX.Left)
			{
				Actor.Flip(); // Flip to always face and walk towards player
			}
		}

		private void unalertedUpdate()
		{
			if (Actor.IsWallAhead.IsColliding())
			{
				Actor.Flip();
			}
		}
	}
	
	// TODO this is literally copy pasted from frog. Find a way to share states between creatures	
	public class JumpState : State<Zombie>
	{
		public Func<bool> LandedOnFloor => () => isLandingOnFloor;
		private bool isLandingOnFloor = false;
		private void landedOnFloor() => isLandingOnFloor = true;
        
		public override void Enter()
		{
			isLandingOnFloor = false;
			Actor.CM.GetComponent<Jump>().AttemptJump();
			Actor.CM.GetComponent<Gravity>().LandedOnFloor += landedOnFloor;
		}
        
		public override void Update( float dt)
		{
			Actor.CM.GetComponent<Move>().Walk(Actor.Facing, dt);
		}
        
		public override void Exit()
		{
			Actor.CM.GetComponent<Gravity>().LandedOnFloor -= landedOnFloor;
		}
	}
	
	protected override void FlipEffect()
	{
		base.FlipEffect();
		IsWallAhead.TargetPosition = new Vector2(-IsWallAhead.TargetPosition.X, IsWallAhead.TargetPosition.Y);
	}

	public override void Die()
	{
		Node2D instance = (Node2D)zombieBodyScene.Instantiate();
		instance.GlobalPosition = GlobalPosition;
		GetParent().CallDeferred(Node.MethodName.AddChild, instance);
		
		base.Die();
	}

	private void _onDetectionAreaPlayerEntered(Player player)
	{
		Alerted = true;
		//CM.GetComponent<Move>().Speed = alertedSpeed;
	}
    
}