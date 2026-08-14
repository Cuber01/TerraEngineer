using Godot;
using System;
using System.Diagnostics;

public partial class HazardRespawnPoint : Area2D
{
	public override void _Ready()
	{
		BodyEntered += playerEntered;
	}

	private void playerEntered(Node2D player)
	{
		Player p = player as Player;
		p!.HazardRespawnPoint = GlobalPosition;
	}
}
