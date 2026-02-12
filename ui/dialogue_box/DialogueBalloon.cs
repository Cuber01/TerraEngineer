using Godot;
using DialogueManagerRuntime;
using TerraEngineer;
using TerraEngineer.game;
using TerraEngineer.ui.player_hud;
using TerraEngineer.ui.textbox;

namespace TENamespace.ui.dialogue_box;

public partial class DialogueBalloon : Node2D, IPopupable
{

	private Resource dialogueResource;
	private StringName startTitle;

	[Export] private PackedScene dialogueButton;
	[Export] private RichTextLabel dialogueLabel;
	[Export] private RichTextLabel nameLabel; 
	[Export] private VBoxContainer choicesContainer;
	[Export] private Node2D decoration;
	
	public Controller Controller { get; set;  }

	private DialogueLine currentLine;
	private bool waitingForChoice = false;

	public override void _Ready()
	{
		choicesContainer.Visible = false;
		SetupControls();
	}

	public void SetupControls()
	{
		Controller = new Controller();
		Controller.AddAction(Names.Actions.Attack, advanceDialogue);
	}

	public override void _Process(double delta)
	{
		Controller.Update((float)delta);
	}
	
	public void PlayDialogue(Resource dialogue, StringName title)
	{
		dialogueResource = dialogue;
		startTitle = title;
		
		Show();
		loadDialogue();
	}

	public void Close()
	{
		Controller.GiveBackControl();
		Hide();
	}

	private async void loadDialogue()
	{
		currentLine = await DialogueManager.GetNextDialogueLine(dialogueResource, startTitle);

		tryDisplayLine();
	}

	private void tryDisplayLine()
	{
		if (currentLine == null)
		{
			Close();
			return;
		}

		// Clear previous choices
		foreach (Node child in choicesContainer.GetChildren())
		{
			child.QueueFree();
		}

		// Choice tree
		if (currentLine.Responses.Count > 0)
		{
			nameLabel.Text = "";
			dialogueLabel.Text = "";
			
			waitingForChoice = true;
			choicesContainer.Visible = true;
			
			// We let godot buttons handle input
			Controller.AddOverride(Names.Actions.Attack, () => {});
			showChoices();
		}
		else
		// Normal dialogue
		{
			nameLabel.Text = currentLine.Character;
			dialogueLabel.Text = currentLine.Text;
			
			waitingForChoice = false;
			choicesContainer.Visible = false;
		}
	}

	private void showChoices()
	{
		int count = 0;
		foreach (DialogueResponse response in currentLine.Responses)
		{
			DialogueButton choiceButton = (DialogueButton)dialogueButton.Instantiate();
			choiceButton.Text = response.Text;
			choiceButton.Pressed += () => onChoiceSelected(response);
			choicesContainer.AddChild(choiceButton);

			if (count == 0)
			{
				choiceButton.GrabFocus();
			}
			count++;
		}
	}

	private async void onChoiceSelected(DialogueResponse response)
	{
		// Hide choices immediately upon selection
		choicesContainer.Visible = false;
		waitingForChoice = false;
		
		// We take back control of input
		Controller.RemoveOverride(Names.Actions.Attack);

		currentLine = await DialogueManager.GetNextDialogueLine(dialogueResource, response.NextId);
		tryDisplayLine();	
	}

	private async void advanceDialogue()
	{
		currentLine = await DialogueManager.GetNextDialogueLine(dialogueResource, currentLine.NextId);
		tryDisplayLine();
	}


}