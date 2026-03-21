using Godot;
using TENamespace.ui.dialogue_box;
using TerraEngineer;

public partial class InteractAndDisplay : Area2D
{
	public enum PopupType
	{
		Dialogue,
		Popup
	}
	
	[Export] private PopupType popupType;
	[Export] private Resource dialogueResource;
	[Export(PropertyHint.MultilineText)] private string popupText;
	[Export] private StringName startTitle;
	
	private Player player;
	private Popup popupTemplate;
	private DialogueBalloon balloonTemplate;

	public override void _Ready()
	{
		player = GetNode<Player>(Names.NodePaths.Player);
		popupTemplate = GetNode<Popup>(Names.NodePaths.Popup);
		balloonTemplate = GetNode<DialogueBalloon>(Names.NodePaths.DialogueBalloon);
	}
	
	private void display()
	{
		if (popupType == PopupType.Dialogue)
		{
			balloonTemplate.PlayDialogue(dialogueResource, startTitle);
			player.Controller.SwitchControl(balloonTemplate.Controller);
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
	}
	
	private void onPlayerExited(Player player)
	{
		player.Controller.RemoveOverride(Names.Actions.Attack);
		player.Interacted -= display;
	}
}
