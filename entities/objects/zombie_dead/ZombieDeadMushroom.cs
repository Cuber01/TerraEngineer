using Godot;
using System;
using TerraEngineer.entities;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.objects;

public partial class ZombieDeadMushroom : Terraformable
{
	[Export] private PackedScene zombieScene;
	[Export] private float reviveTime = 5;
	private ITimer reviveTimer;
	
	public override void Enable()
	{
		base.Enable();
		reviveTimer = TimerManager.Schedule(reviveTime, this, (_) => CallDeferred(nameof(onRevive)));
	}

	public override void Disable()
	{
		base.Disable();
		TimerManager.Cancel(reviveTimer);
	}
	
	private void onRevive()
	{
		Zombie instance = (Zombie)zombieScene.Instantiate();
		instance.GlobalPosition = new Vector2(GlobalPosition.X , GlobalPosition.Y);
		Caretaker.GetParent().AddChild(instance);
		Caretaker.QueueFree();
	}
}
