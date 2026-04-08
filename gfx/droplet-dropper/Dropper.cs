using Godot;
using System;

public partial class Dropper : Node2D
{
	[Export] private PackedScene dropletScene;
	[Export] private Node2D spawnPlace;
	[Export] private AnimatedSprite2D sprite2D;
	
	public override void _Ready()
	{
		sprite2D.Play("default");
		sprite2D.AnimationLooped += spawnDroplet;
	}

	private void spawnDroplet()
	{
		Droplet instance = (Droplet)dropletScene.Instantiate();
		//instance.GlobalPosition = spawnPlace.GlobalPosition;
		instance.Position =  spawnPlace.Position;
		AddChild(instance);
	}
}
