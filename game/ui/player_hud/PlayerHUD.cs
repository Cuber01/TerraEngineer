using Godot;
using System;
using TerraEngineer.game;
using TerraEngineer.ui.player_hud;

public partial class PlayerHUD : Node2D, IConnectable<Player>
{
	
	private void ApplyToChildren(Player player, Action<IConnectable<Player>, Player> action)
	{
		foreach (var child in GetChildren())
		{
			if (child is IConnectable<Player> element)
			{
				action(element, player);
			}
		}
	}

	public void Connect(Player player)
	{
		player.OpenMap += Hide;
		player.CloseMap += Show;
		player.OpenInventory += Hide;
		player.CloseInventory += Show;
		ApplyToChildren(player, (e, p) => e.Connect(player));
	}

	public void Disconnect(Player player)
	{
		player.OpenMap -= Hide;
		player.OpenInventory -= Hide;
		player.CloseMap -= Show;
		player.CloseInventory -= Show;
		ApplyToChildren(player, (e, p) => e.Disconnect(player));
	}

}
