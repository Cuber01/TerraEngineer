using DialogueManagerRuntime;
using Godot;
using TENamespace.player_inventory;
using TENamespace.ui.dialogue_box;
using TerraEngineer.game.ui;


namespace TerraEngineer.entities.objects.puzzle.plant;

public partial class PuzzlePlantForest : TerraformableEntity, IInteractable
{
    [Export] private Resource dialogue;
    [Export] private PackedScene mushroomCapScene;
    
    private DialogueBalloon balloonTemplate;
    private Player player;
    private Node2D room;
    private PickupableItem mushroomCap;
    
    private bool hasBeenWatered = false;
    
    public bool InteractionBlocked { get; set; }

    public override void _Ready()
    {
        base._Ready();
        
        SpriteWrapper.Init(Sprite);
        
        player = GetNode<Player>(Names.NodePaths.Player);
        balloonTemplate = GetNode<DialogueBalloon>(Names.NodePaths.DialogueBalloon);
        room = GetParent().GetParent().GetParent<Node2D>();
    }

    public void OnInteracted()
    {
        bool hasWateringCan = player.CM.GetComponent<PlayerInventory>().HasItem("watering_can");
        
        StringName dialogueTitle;
        
        if (hasBeenWatered)
        {
            dialogueTitle = "already_watered";
        }
        else if (hasWateringCan)
        {
            dialogueTitle = "has_watering_can";
        }
        else
        {
            dialogueTitle = "no_watering_can";
        }
        
        balloonTemplate.PlayDialogue(dialogue, dialogueTitle);
        InputStackManager.Push(balloonTemplate.InputContext);
        
        DialogueManager.DialogueEnded += onDialogueEnded;
    }
    
    private void onDialogueEnded(Resource dialogueResource)
    {
        DialogueManager.DialogueEnded -= onDialogueEnded;
        
        
        int choice = GlobalDialoguesState.Instance.PuzzlePlant_Choice;
        GlobalDialoguesState.Instance.PuzzlePlant_Choice = 0;
        
        if (choice == 1 && !hasBeenWatered)
        {
            // Player chose to water the plant
            hasBeenWatered = true;
            SpriteWrapper.Play("grown");
            
            // Spawn mushroom cap pickup
            mushroomCap = mushroomCapScene.Instantiate<PickupableItem>();
            mushroomCap.GlobalPosition = GlobalPosition;
            disableCap();
            room.AddChild(mushroomCap);
            Caretaker.Terraformed += terraformCap;
        }
    }

    private void terraformCap(Biomes biome)
    {
        if (biome == Biomes.Mushroom)
        {
            enableCap();
        }
        else
        {
            disableCap();
        }
    }

    private void disableCap()
    {
        mushroomCap.ProcessMode = ProcessModeEnum.Disabled;
        mushroomCap.Visible = false;
    }

    private void enableCap()
    {
        mushroomCap.ProcessMode = ProcessModeEnum.Pausable;
        mushroomCap.Visible = true;
    }
}
