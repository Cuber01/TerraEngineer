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
	
	public Controller controller = new();

	public override void Init()
	{
		controller.AddAction(Names.Actions.Weapon0, () => CM.GetComponent<GunHandle>().ChangeWeapon(0), Names.Actions.GroupWeapon);
		controller.AddAction(Names.Actions.Weapon1, () => CM.GetComponent<GunHandle>().ChangeWeapon(1), Names.Actions.GroupWeapon);
		controller.AddAction(Names.Actions.Weapon2, () => CM.GetComponent<GunHandle>().ChangeWeapon(2), Names.Actions.GroupWeapon);
		controller.AddAction(Names.Actions.Weapon3, () => CM.GetComponent<GunHandle>().ChangeWeapon(3), Names.Actions.GroupWeapon);
		controller.AddAction(Names.Actions.WeaponNext, () => CM.GetComponent<GunHandle>().ChangeToNextWeapon(), Names.Actions.GroupWeapon);
		controller.AddAction(Names.Actions.GunHandleNext, () => CM.GetComponent<GunHandle>().ChangeGunHandle(), Names.Actions.GroupWeapon);
		controller.AddAction(Names.Actions.Interact, () => Interacted?.Invoke());
	}
	
	public override void _PhysicsProcess(double delta)
	{
		DirectionX moveDir = (DirectionX)(int)Input.GetAxis("ui_left", "ui_right");
		if (moveDir != 0)
		{
			Facing = moveDir;
		}
		
		if (Input.IsActionJustPressed("dash"))
		{
			CM.GetComponent<Dash>().AttemptDash(Facing);
		}
		
		controller.Update((float)delta);
		
		CM.GetComponent<Move>().Walk(moveDir, (float)delta);
		CM.UpdateComponents((float)delta);
		
		if (Input.IsActionJustPressed("jump"))
		{
			CM.GetComponent<Jump>().AttemptJump();
		}
		
		if (Input.IsActionJustReleased("jump")) {
			CM.GetComponent<Jump>().LimitJump();
		}
		
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
			CallDeferred(Node.MethodName.QueueFree);    
		}
		Dead = true;
	}

	private void onBumpedCeiling(Node2D body)
	{
		CM.GetComponent<Jump>().CancelJump();
	}

	
	// Wrapper for gdscript
	public void ActivateInventory()
	 => CM.GetComponent<PlayerInventory>().ActivateItems(this);
}


