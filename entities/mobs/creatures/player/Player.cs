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

public enum PlayerTriggers
{
	PressedMove,
	ReleasedMove,
	PressedJump,
	PressedDash,
	Landed
}

public partial class Player : Creature
{
	public delegate void InteractedEventHandler();
	public event InteractedEventHandler Interacted;

	private readonly DashState dashState = new DashState();
	private readonly JumpState jumpState = new JumpState();
	private readonly WalkState walkState = new WalkState();
	private readonly IdleState idleState = new IdleState();
	private readonly NoclipState noclipState = new NoclipState();
	
	private StateMachineWithTriggers<Player, PlayerTriggers> fsm;
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

		fsm = new StateMachineWithTriggers<Player, PlayerTriggers>(this, idleState, true);
		fsm.AddTransition(idleState, walkState, () => fsm.IsTriggered(PlayerTriggers.PressedMove));
		fsm.AddTransition(walkState, idleState, () => fsm.IsTriggered(PlayerTriggers.ReleasedMove));
		
		fsm.AddTransition(jumpState, idleState, () => fsm.IsTriggered(PlayerTriggers.Landed));
		fsm.AddTransition(jumpState, walkState, () => fsm.IsTriggered(PlayerTriggers.Landed) &&
		                                              fsm.IsTriggered(PlayerTriggers.PressedMove)
													  , 1);
		
		fsm.AddTransition(idleState, jumpState, () => fsm.IsTriggered(PlayerTriggers.PressedJump) && IsOnFloor());
		fsm.AddTransition(walkState, jumpState, () => fsm.IsTriggered(PlayerTriggers.PressedJump) && IsOnFloor());

		fsm.AddTransition(dashState, jumpState, () => CM.GetComponent<Dash>().IsDashing);
		fsm.AddTransition(jumpState, dashState, () => fsm.IsTriggered(PlayerTriggers.PressedDash));
		fsm.AddTransition(walkState, dashState, () => fsm.IsTriggered(PlayerTriggers.PressedDash));
		fsm.AddTransition(idleState, dashState, () => fsm.IsTriggered(PlayerTriggers.PressedDash));
		
		CM.GetComponent<Gravity>().LandedOnFloor += () => fsm.FireTrigger(PlayerTriggers.Landed);
		
		Controller.AddAction(Names.Actions.Dash, () =>
		{
			fsm.FireTrigger(PlayerTriggers.PressedDash);
		});
		
		Controller.AddAction(Names.Actions.Jump, () =>
		{
			fsm.FireTrigger(PlayerTriggers.PressedJump);
		});
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
				Actor.fsm.FireTrigger(PlayerTriggers.PressedMove);
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
				Actor.fsm.FireTrigger(PlayerTriggers.ReleasedMove);
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
		}
		
		public override void Update(float dt)
		{
			DirectionX moveDir = Actor.Controller.GetAxis("ui_left", "ui_right");
			if (moveDir != 0)
			{
				Actor.Flip(moveDir);
				Actor.fsm.FireTrigger(PlayerTriggers.PressedMove);
			} else if (Actor.fsm.IsTriggered(PlayerTriggers.PressedMove))
			{
				Actor.fsm.FireTrigger(PlayerTriggers.ReleasedMove);
			}
			
			if(Actor.fsm.IsTriggered(PlayerTriggers.PressedJump))
				Actor.CM.GetComponent<Jump>().AttemptJump();
			
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
	}
	
	public class DashState : State<Player>
	{
		public override void Enter()
		{
			#if DEBUG_STATE
			GD.Print("Entered Dash.");
			#endif

			Actor.CM.GetComponent<Dash>().AttemptDash(Actor.Facing);
			Actor.SpriteWrapper.Play(Names.Animations.Dash);
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


