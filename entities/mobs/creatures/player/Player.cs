using Godot;
using System;
using System.Numerics;
using TENamespace;
using TENamespace.health;
using TENamespace.player_inventory;
using TENamespace.projectile_builder;
using TerraEngineer;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.mobs.creatures;
using TerraEngineer.game;
using Vector2 = Godot.Vector2;

public partial class Player : Creature
{
	public delegate void InteractedEventHandler();
	public event InteractedEventHandler Interacted;

	private readonly DashState dashState = new DashState();
	private readonly JumpState jumpState = new JumpState();
	private readonly WalkState walkState = new WalkState();
	private readonly IdleState idleState = new IdleState();
	private readonly NoclipState noclipState = new NoclipState();
	
	private StateMachine<Player> fsm;
	public Controller Controller = new();

	private bool updateFrozen = false;
	private const float RoomTransitionForce = 10f;
	private const float RoomTransitionForceUpModifier = 3f;
	
	public override void Init()
	{
		Controller.AddAction(Names.Actions.Weapon0, () => CM.GetComponent<GunHandle>().ChangeWeapon(0), Names.Actions.GroupWeapon);
		Controller.AddAction(Names.Actions.Weapon1, () => CM.GetComponent<GunHandle>().ChangeWeapon(1), Names.Actions.GroupWeapon);
		Controller.AddAction(Names.Actions.Weapon2, () => CM.GetComponent<GunHandle>().ChangeWeapon(2), Names.Actions.GroupWeapon);
		Controller.AddAction(Names.Actions.Weapon3, () => CM.GetComponent<GunHandle>().ChangeWeapon(3), Names.Actions.GroupWeapon);
		Controller.AddAction(Names.Actions.WeaponNext, () => CM.GetComponent<GunHandle>().ChangeToNextWeapon(), Names.Actions.GroupWeapon);
		Controller.AddAction(Names.Actions.GunHandleNext, () => CM.GetComponent<GunHandle>().ChangeGunHandle(), Names.Actions.GroupWeapon);
		Controller.AddReleaseAction(Names.Actions.Jump, () => CM.GetComponent<Jump>().LimitJump());
		
		// State machine related
		fsm = new StateMachine<Player>(this, idleState, true);
		fsm.AddTransition(idleState, walkState, idleState.CheckWalkState);
		fsm.AddTransition(walkState, idleState, checkIdleState);
		fsm.AddTransition(jumpState, idleState, checkIdleState);
		fsm.AddTransition(walkState, jumpState, checkJumpState);
		fsm.AddTransition(idleState, jumpState, checkJumpState);
		fsm.AddTransition(jumpState, walkState, jumpState.CheckWalkState);
		fsm.AddTransition(dashState, jumpState, dashState.CheckJumpState);
		fsm.AddTransition(jumpState, dashState, checkDashState);
		fsm.AddTransition(walkState, dashState, checkDashState);
		fsm.AddTransition(idleState, dashState, checkDashState);
		
		Controller.AddAction(Names.Actions.Dash, () =>
		{
			CM.GetComponent<Dash>().AttemptDash(Facing);
		});
		
		Controller.AddAction(Names.Actions.Jump, () =>
		{
			if (fsm.CurrentState != dashState)
				CM.GetComponent<Jump>().AttemptJump();
		});
	}
	
	private bool checkIdleState()
	{
		if (IsOnFloor() && velocity.Abs() < Vector2.One)
			return true;
		else
			return false;
	}

	private bool checkJumpState()
	{
		return !IsOnFloor();
	}

	private bool checkDashState()
	{
		return CM.GetComponent<Dash>().IsDashing;
	}
	
	public class IdleState : State<Player>
	{
		public override void Enter()
		{
			#if DEBUG_STATE
			GD.Print("Entered Idle.");
			#endif
		}

		public bool CheckWalkState()
		{
			DirectionX moveDir = Actor.Controller.GetAxis("ui_left", "ui_right");
			if (moveDir != 0)
			{
				Actor.Facing = moveDir;
				return true;
			}
			return false;
		}
	}
	
	public class WalkState : State<Player>
	{
		public override void Enter()
		{
			#if DEBUG_STATE
			GD.Print("Entered Walk.");
			#endif
		}
		
		public override void Update( float dt)
		{
			DirectionX moveDir = Actor.Controller.GetAxis("ui_left", "ui_right");
			if (moveDir != 0)
			{
				Actor.Facing = moveDir;
			}
			Actor.CM.GetComponent<Move>().Walk(moveDir, dt);
		}
	}
	
	public class JumpState : State<Player>
	{
		public override void Enter()
		{
			#if DEBUG_STATE
			GD.Print("Entered Jump.");
			#endif
		}
		
		public override void Update(float dt)
		{
			DirectionX moveDir = Actor.Controller.GetAxis("ui_left", "ui_right");
			if (moveDir != 0)
			{
				Actor.Facing = moveDir;
			}
			Actor.CM.GetComponent<Move>().Walk(moveDir, dt);
		}
		
		public bool CheckWalkState()
		{
			if (Actor.IsOnFloor() && MathF.Abs(Actor.velocity.X) >= 1)
				return true;
			else
				return false;
		}
	}
	
	public class DashState : State<Player>
	{
		public override void Enter()
		{
			#if DEBUG_STATE
			GD.Print("Entered Dash.");
			#endif
		}
		
		public bool CheckJumpState()
		{
			return !Actor.CM.GetComponent<Dash>().IsDashing;
		}
		
		public override void Update(float dt)
		{
			
		}
		
	}
	
	public class NoclipState : State<Player>
	{
		public override void Enter()
		{
			Actor.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
			Actor.CM.GetComponent<Gravity>().Disabled = true;
		}

		public override void Update( float dt)
		{
			Vector2 dir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
			if (dir != Vector2.Zero)
			{
				Actor.Facing = (DirectionX)dir.X;
			}
		
			if (Input.IsActionJustPressed("dash"))
			{
				Actor.CM.GetComponent<Dash>().AttemptDash(Actor.Facing);
			}
		
			Actor.Controller.Update((float)dt);

			Actor.velocity = dir * 150;
		}

		public override void Exit()
		{
			Actor.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
			Actor.CM.GetComponent<Gravity>().Disabled = false;
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if(updateFrozen) return;
		
		#if DEBUG
		if(Input.IsActionJustPressed("f2"))
		{
			if (fsm.CurrentState is NoclipState)
			{
				fsm.ChangeState(idleState);
			}
			else
			{
				fsm.ChangeState(noclipState);	
			}
		}
		
		if(Input.IsActionJustPressed("f3"))
		{
			GodMode = !GodMode;
		}
		#endif
		
		Controller.Update((float)delta);
		fsm.Update((float)delta);
		CM.UpdateComponents((float)delta);

		Velocity = velocity;
		MoveAndSlide();
	}

	public Direction4 GetShootDirection()
	{
		Vector2 vector = Input.GetVector("ui_left", "ui_right", 
										 "ui_down", "ui_up");
		if (vector.Y > 0) return Direction4.Up;
		if (vector.Y < 0) return Direction4.Down;
		if (vector.X > 0) return Direction4.Right;
		if (vector.X < 0) return Direction4.Left;

		return (Direction4)(int)Facing;
	}

	public override void Die()
	{
		if (!Dead)
		{
			GetTree().ReloadCurrentScene();
		}
		Dead = true;
	}

	#region Callbacks
	
	private void onBumpedCeiling(Node2D body)
	{
		CM.GetComponent<Jump>().CancelJump();
	}
	
	public void onRoomLoaded(Node level) => updateFrozen = false;

	private void onRoomEntered(string roomName, Vector2I playerDirection)
	{
		float extraForce = RoomTransitionForce;
		if (playerDirection == Vector2I.Up)
			extraForce *= RoomTransitionForceUpModifier;
		
		velocity += extraForce * (Vector2)(playerDirection);
		updateFrozen = true;
	}
	
	public void InvokeInteracted() => Interacted?.Invoke();
	
	// Wrapper for gdscript
	public void ActivateInventory() => CM.GetComponent<PlayerInventory>().ActivateItems(this);
	
	#endregion
}


