using Godot;
using TENamespace.ui.dialogue_box;
using TerraEngineer;
using TerraEngineer.entities.objects;
using TerraEngineer.game.ui;

public partial class InteractAndDisplay : Area2D, IInteractable
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
	
	public bool InteractionBlocked { get; set; }
	
	private Player player;
	private Popup popupTemplate;
	private DialogueBalloon balloonTemplate;

	public override void _Ready()
	{
		player = GetNode<Player>(Names.NodePaths.Player);
		popupTemplate = GetNode<Popup>(Names.NodePaths.Popup);
		balloonTemplate = GetNode<DialogueBalloon>(Names.NodePaths.DialogueBalloon);
	}

	public void OnInteracted()
	{
		if (popupType == PopupType.Dialogue)
		{
			balloonTemplate.PlayDialogue(dialogueResource, startTitle);
			InputStackManager.Push(balloonTemplate.InputContext);
		}
		else if (popupType == PopupType.Popup)
		{
			popupTemplate.ShowPopup(popupText);
			InputStackManager.Push(popupTemplate.InputContext);
		}
	}
}
