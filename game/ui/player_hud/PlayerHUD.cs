using Godot;
using System;
using TerraEngineer.game;
using TerraEngineer.game.ui;
using TerraEngineer.ui.player_hud;

public partial class PlayerHUD : Node2D, IUserInterface
{
	public bool IsOpen { get; set; }
	
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
		ApplyToChildren(player, (e, p) => e.Connect(player));
	}
	
	public void Disconnect(Player player)
	{
		ApplyToChildren(player, (e, p) => e.Disconnect(player));
	}
	
	public void Open()
	{
		Show();
		IsOpen = true;
	}
	
	public void Close()
	{
		Hide();
		IsOpen = false;
	}
}
