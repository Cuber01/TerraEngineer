using Godot;
using System;
using TerraEngineer;
using TerraEngineer.ui.textbox;

public partial class InteractAndDisplay : Area2D
{
	private Player player;
	[Export] private Node2D displayer;

	private void display()
	{
		IPopupable popup = (IPopupable)displayer;
		player.Controller.SwitchControl(popup.Controller);
		popup.Display();
	}

	private void onPlayerEntered(Player player)
	{
		player.Controller.AddOverride(Names.Actions.Attack, player.InvokeInteracted);
		player.Interacted += display;
		this.player = player;
	}
	
	private void onPlayerExited(Player player)
	{
		player.Controller.RemoveOverride(Names.Actions.Attack);
		player.Interacted -= display;
	}
}
