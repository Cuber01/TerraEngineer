using Godot;
using System;
using TENamespace;
using TENamespace.health;
using TENamespace.player_inventory;
using TENamespace.projectile_builder;
using TerraEngineer;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.mobs.creatures;
using TerraEngineer.game;

public partial class Player : Creature
{
	public delegate void InteractedEventHandler();
	public event InteractedEventHandler Interacted;

	private readonly DashState dashState = new DashState();
	private readonly JumpState walkState = new JumpState();
	private readonly WalkState jumpState = new WalkState();
	private readonly IdleState idleState = new IdleState();
	private readonly NoclipState noclipState = new NoclipState();
	
	private StateMachine<Player> fsm;
	public Controller controller = new();
	
	public class IdleState : IState<Player>
	{
		public void Enter(Player actor)
		{
		}

		public void Update(Player actor, float dt)
		{
			DirectionX moveDir = (DirectionX)(int)Input.GetAxis("ui_left", "ui_right");
			if (moveDir != 0)
			{
				actor.Facing = moveDir;
			}
		
			if (Input.IsActionJustPressed("dash"))
			{
				actor.CM.GetComponent<Dash>().AttemptDash(actor.Facing);
			}
		
			actor.controller.Update((float)dt);
		
			actor.CM.GetComponent<Move>().Walk(moveDir, (float)dt);
			actor.CM.UpdateComponents((float)dt);
		
			if (Input.IsActionJustPressed("jump"))
			{
				actor.CM.GetComponent<Jump>().AttemptJump();
			}
		
			if (Input.IsActionJustReleased("jump")) {
				actor.CM.GetComponent<Jump>().LimitJump();
			}
		}

		public void Exit(Player actor)
		{
		}
	}

	public class DashState : IState<Player>
	{
		public void Enter(Player actor)
		{
			throw new NotImplementedException();
		}

		public void Update(Player actor, float dt)
		{
			throw new NotImplementedException();
		}

		public void Exit(Player actor)
		{
			throw new NotImplementedException();
		}
	}

	public class WalkState : IState<Player>
	{
		public void Enter(Player actor)
		{
			throw new NotImplementedException();
		}

		public void Update(Player actor, float dt)
		{
			throw new NotImplementedException();
		}

		public void Exit(Player actor)
		{
			throw new NotImplementedException();
		}
	}
	
	public class NoclipState : IState<Player>
	{
		public void Enter(Player actor)
		{
			actor.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
			actor.CM.GetComponent<Gravity>().Disabled = true;
		}

		public void Update(Player actor, float dt)
		{
			Vector2 dir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
			if (dir != Vector2.Zero)
			{
				actor.Facing = (DirectionX)dir.X;
			}
		
			if (Input.IsActionJustPressed("dash"))
			{
				actor.CM.GetComponent<Dash>().AttemptDash(actor.Facing);
			}
		
			actor.controller.Update((float)dt);

			actor.velocity = dir * 150;
		}

		public void Exit(Player actor)
		{
			actor.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
			actor.CM.GetComponent<Gravity>().Disabled = false;
		}
	}
	
	public class JumpState : IState<Player>
	{
		public void Enter(Player actor)
		{
			throw new NotImplementedException();
		}

		public void Update(Player actor, float dt)
		{
			throw new NotImplementedException();
		}

		public void Exit(Player actor)
		{
			throw new NotImplementedException();
		}
	}
	
	public override void Init()
	{
		controller.AddAction(Names.Actions.Weapon0, () => CM.GetComponent<GunHandle>().ChangeWeapon(0), Names.Actions.GroupWeapon);
		controller.AddAction(Names.Actions.Weapon1, () => CM.GetComponent<GunHandle>().ChangeWeapon(1), Names.Actions.GroupWeapon);
		controller.AddAction(Names.Actions.Weapon2, () => CM.GetComponent<GunHandle>().ChangeWeapon(2), Names.Actions.GroupWeapon);
		controller.AddAction(Names.Actions.Weapon3, () => CM.GetComponent<GunHandle>().ChangeWeapon(3), Names.Actions.GroupWeapon);
		controller.AddAction(Names.Actions.WeaponNext, () => CM.GetComponent<GunHandle>().ChangeToNextWeapon(), Names.Actions.GroupWeapon);
		controller.AddAction(Names.Actions.GunHandleNext, () => CM.GetComponent<GunHandle>().ChangeGunHandle(), Names.Actions.GroupWeapon);
		fsm = new StateMachine<Player>(this, idleState, true);
	}
	
	public override void _PhysicsProcess(double delta)
	{
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

	private void onBumpedCeiling(Node2D body)
	{
		CM.GetComponent<Jump>().CancelJump();
	}

	public void InvokeInteracted() => Interacted?.Invoke();
	
	// Wrapper for gdscript
	public void ActivateInventory()
	 => CM.GetComponent<PlayerInventory>().ActivateItems(this);
}


