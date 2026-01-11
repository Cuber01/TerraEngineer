using Godot;
using System;
using TerraEngineer;
using TerraEngineer.ui.textbox;

public partial class InteractAndDisplay : Area2D
{
	[Export] private Node2D displayer;

	private void display()
	{
		((IPopupable)displayer).Display();
	}

	private void onPlayerEntered(Player player)
	{
		player.Controller.AddOverride(Names.Actions.Attack, player.InvokeInteracted);
		player.Interacted += display;
	}
	
	private void onPlayerExited(Player player)
	{
		player.Controller.RemoveOverride(Names.Actions.Attack);
		player.Interacted -= display;
	}
}
