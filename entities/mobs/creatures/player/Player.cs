
#define DEBUG_STATE
using DialogueManagerRuntime;
using Godot;
using System;
using TENamespace;
using TENamespace.player_inventory;
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
	public bool IsJumping = false;
	
	
	public override void Init()
	{
		
		Controller.TurnActive = true;
		Controller.AddAction(Names.Actions.Weapon0, () => CM.GetComponent<GunHandle>().ChangeWeapon(0), Names.Actions.GroupWeapon);
		Controller.AddAction(Names.Actions.Weapon1, () => CM.GetComponent<GunHandle>().ChangeWeapon(1), Names.Actions.GroupWeapon);
		Controller.AddAction(Names.Actions.Weapon2, () => CM.GetComponent<GunHandle>().ChangeWeapon(2), Names.Actions.GroupWeapon);
		Controller.AddAction(Names.Actions.Weapon3, () => CM.GetComponent<GunHandle>().ChangeWeapon(3), Names.Actions.GroupWeapon);
		Controller.AddAction(Names.Actions.WeaponNext, () => CM.GetComponent<GunHandle>().ChangeToNextWeapon(), Names.Actions.GroupWeapon);
		Controller.AddAction(Names.Actions.GunHandleNext, () => CM.GetComponent<GunHandle>().ChangeGunHandle(), Names.Actions.GroupWeapon);
		Controller.AddReleaseAction(Names.Actions.Jump, () => CM.GetComponent<Jump>().LimitJump());
		
		// State machine related
		fsm = new StateMachine<Player>(this, idleState, true);
		//fsm.AddTransition(idleState, walkState, idleState.CheckWalkState);
		//fsm.AddTransition(walkState, idleState, checkIdleState);
		//fsm.AddTransition(jumpState, idleState, checkIdleState);
		//fsm.AddTransition(idleState, jumpState, checkJumpState);
		//fsm.AddTransition(jumpState, walkState, jumpState.CheckWalkState);
		//fsm.AddTransition(dashState, jumpState, dashState.CheckJumpState);
		//fsm.AddTransition(jumpState, dashState, checkDashState);
		//fsm.AddTransition(walkState, dashState, checkDashState);
		//fsm.AddTransition(idleState, dashState, checkDashState);
		
		Controller.AddAction(Names.Actions.Dash, () =>
		{
			//CM.GetComponent<Dash>().AttemptDash(Facing);
		});
		
		Controller.AddAction(Names.Actions.Jump, () =>
		{
			fsm.ChangeState(jumpState);
		});
	}
	
	private bool checkIdleState()
	{
		if (IsOnFloor() && Controller.GetAxis("ui_left", "ui_right") == 0 && fsm.CurrentState != jumpState)
			return true;
		else
			return false;
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
			
			Actor.SpriteWrapper.Play(Names.Animations.Idle);
		}

		public override void Update(float dt)
		{
			DirectionX moveDir = Actor.Controller.GetAxis("ui_left", "ui_right");
			if (moveDir != 0)
			{
				Actor.fsm.ChangeState(Actor.walkState);
			}
		}
		
	}
	
	public class WalkState : State<Player>
	{
		public override void Enter()
		{
			#if DEBUG_STATE
			GD.Print("Entered Walk.");
			#endif
			
			Actor.SpriteWrapper.Play(Names.Animations.Walk);
		}
		
		public override void Update( float dt)
		{
			DirectionX moveDir = Actor.Controller.GetAxis("ui_left", "ui_right");
			if (moveDir != 0)
			{
				Actor.Flip(moveDir);
			}
			else
			{
				Actor.fsm.ChangeState(Actor.idleState);
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

			Actor.SpriteWrapper.AnimationFinished += afterStartJump;
			Actor.SpriteWrapper.Play(Names.Animations.Jump);
			
			Actor.CM.GetComponent<Jump>().AttemptJump();
			Actor.CM.GetComponent<Gravity>().LandedOnFloor += landed;
		}
		
		public override void Update(float dt)
		{
			DirectionX moveDir = Actor.Controller.GetAxis("ui_left", "ui_right");
			if (moveDir != 0)
			{
				Actor.Flip(moveDir);
			}
			Actor.CM.GetComponent<Move>().Walk(moveDir, dt);
		}

		public override void Exit()
		{
			Actor.SpriteWrapper.Play(Names.Animations.Land);
		}

		private void afterStartJump()
		{
			Actor.SpriteWrapper.AnimationFinished -= afterStartJump;
			Actor.SpriteWrapper.Play(Names.Animations.Fly);
		}

		private void landed()
		{
			if (MathF.Abs(Actor.velocity.X) >= 1)
			{ 
				Actor.fsm.ChangeState(Actor.walkState);	
			}
			else
			{ 
				Actor.fsm.ChangeState(Actor.idleState);		
			}
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
		
		if(Input.IsActionJustPressed("f4"))
		{
			DialogueManager.ShowDialogueBalloon(ResourceLoader.Load("res://dialogue/radio01.dialogue"), "start");
		}
		#endif
		
		Controller.Update((float)delta);
		fsm.Update((float)delta);
		CM.UpdateComponents((float)delta);
		HandleMove();
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
			GetTree().CallDeferred(SceneTree.MethodName.ReloadCurrentScene);
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
	
	public void Freeze() => updateFrozen = true;
	public void Unfreeze() => updateFrozen = false;
	
	#endregion
}


