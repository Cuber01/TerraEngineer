using DialogueManagerRuntime;
using Godot;
using TENamespace.player_inventory;
using TENamespace.ui.dialogue_box;
using TerraEngineer;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.objects;
using TerraEngineer.game.sprite;
using TerraEngineer.game.ui;

public partial class CraftingStation : Entity, IInteractable
{
    [Export] private Resource dialogue;

    private static readonly StringName[] RequiredItems =
    {
        "gunpowder",
        "ice_crystal",
        "mushroom_cap",
        "vine",
    };

    private static readonly StringName CraftedItem = "bomb";

    private DialogueBalloon balloonTemplate;
    private Player player;

    public bool InteractionBlocked { get; set; }

    public override void _Ready()
    {
        balloonTemplate = GetNode<DialogueBalloon>(Names.NodePaths.DialogueBalloon);
        player = GetNode<Player>(Names.NodePaths.Player);
    }

    public void OnInteracted()
    {
        StringName dialogueTitle;

        if (hasAllRequiredItems())
        {
            dialogueTitle = "has_all_items";
        }
        else
        {
            dialogueTitle = "missing_items";
        }

        balloonTemplate.PlayDialogue(dialogue, dialogueTitle);
        InputStackManager.Push(balloonTemplate.InputContext);

        DialogueManager.DialogueEnded += onDialogueEnded;
    }

    private bool hasAllRequiredItems()
    {
        PlayerInventory inventory = player.CM.GetComponent<PlayerInventory>();
        foreach (StringName item in RequiredItems)
        {
            if (!inventory.HasItem(item))
            {
                return false;
            }
        }

        return true;
    }

    private void onDialogueEnded(Resource dialogueResource)
    {
        DialogueManager.DialogueEnded -= onDialogueEnded;

        int choice = GlobalDialoguesState.Instance.Lab_CraftingStation;
        GlobalDialoguesState.Instance.Lab_CraftingStation = 0;

        if (choice == 1)
        {
            craftBomb();
        }
    }

    private void craftBomb()
    {
        PlayerInventory inventory = player.CM.GetComponent<PlayerInventory>();
        if (!inventory.TryAddUniqueItem(CraftedItem))
        {
            return;
        }

        foreach (StringName item in RequiredItems)
        {
            inventory.RemoveUniqueItem(item);
        }
    }
}


