using Godot;
using System;
using TENamespace.ui.dialogue_box;
using TerraEngineer;
using TerraEngineer.ui.textbox;

public partial class InteractAndDisplay : Area2D
{
	public enum PopupType
	{
		Dialogue,
		Popup
	}
	
	[Export] private PopupType popupType;
	[Export] private Resource dialogueResource;
	[Export] private StringName popupText;
	[Export] private StringName startTitle;
	
	private Player player;
	private Popup popupTemplate;
	private DialogueBalloon baloonTemplate;

	public override void _Ready()
	{
		popupTemplate = GetNode<Popup>(Names.NodePaths.Popup);
		baloonTemplate = GetNode<DialogueBalloon>(Names.NodePaths.DialogueBalloon);
	}
	
	private void display()
	{
		if (popupType == PopupType.Dialogue)
		{
			baloonTemplate.PlayDialogue(dialogueResource, startTitle);
			player.Controller.SwitchControl(baloonTemplate.Controller);
		}
		else if (popupType == PopupType.Popup)
		{
			popupTemplate.ShowPopup(popupText);
			player.Controller.SwitchControl(popupTemplate.Controller);
		}
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
