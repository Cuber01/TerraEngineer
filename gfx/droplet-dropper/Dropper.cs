using Godot;
using System;
using TerraEngineer;

public partial class Dropper : Node2D
{
	[Export] private PackedScene dropletScene;
	[Export] private Node2D spawnPlace;
	[Export] private AnimatedSprite2D sprite2D;
	
	public override void _Ready()
	{
		sprite2D.Play("default", MathT.RandomFloat(1,10f));
		sprite2D.AnimationLooped += spawnDroplet;
	}

	private void spawnDroplet()
	{
		Droplet instance = (Droplet)dropletScene.Instantiate();
		instance.Position =  spawnPlace.Position;
		AddChild(instance);
	}
}
