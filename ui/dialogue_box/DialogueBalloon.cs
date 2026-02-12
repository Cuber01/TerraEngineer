using Godot;
using DialogueManagerRuntime;
using TerraEngineer;
using TerraEngineer.game;
using TerraEngineer.ui.textbox;

namespace TENamespace.ui.dialogue_box;

public partial class DialogueBalloon : Node2D, IPopupable
{
	[Export] private Resource dialogueResource;
	[Export] private string startTitle = "start";

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
	
	public void Display()
	{
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
		
		nameLabel.Text = currentLine.Character;
		dialogueLabel.Text = currentLine.Text;

		// Clear previous choices
		foreach (Node child in choicesContainer.GetChildren())
		{
			child.QueueFree();
		}

		// Check if this line has branching responses
		if (currentLine.Responses.Count > 0)
		{
			waitingForChoice = true;
			choicesContainer.Visible = true;
			showChoices();
		}
		else
		{
			waitingForChoice = false;
			choicesContainer.Visible = false;
		}
	}

	private void showChoices()
	{
		foreach (DialogueResponse response in currentLine.Responses)
		{
			DialogueButton choiceButton = (DialogueButton)dialogueButton.Instantiate();
			choiceButton.Text = response.Text;
			choiceButton.Pressed += () => onChoiceSelected(response);
			choicesContainer.AddChild(choiceButton);
		}
	}

	private async void onChoiceSelected(DialogueResponse response)
	{
		// Hide choices immediately upon selection
		choicesContainer.Visible = false;
		waitingForChoice = false;

		currentLine = await DialogueManager.GetNextDialogueLine(dialogueResource, response.NextId);
		tryDisplayLine();	
	}

	private async void advanceDialogue()
	{
		currentLine = await DialogueManager.GetNextDialogueLine(dialogueResource, currentLine.NextId);
		tryDisplayLine();
	}
}