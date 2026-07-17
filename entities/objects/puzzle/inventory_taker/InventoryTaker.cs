using DialogueManagerRuntime;
using Godot;
using System.Collections.Generic;
using TENamespace.player_inventory;
using TENamespace.save_entity;
using TENamespace.ui.dialogue_box;
using TerraEngineer;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.objects;
using TerraEngineer.entities.objects.puzzle;
using TerraEngineer.game.sprite;

public partial class InventoryTaker : Entity, IInteractable, ISwitcher
{
    [Export] private StringName[] requiredItems;
    [Export] private Resource dialogue;
    
    private DialogueBalloon balloonTemplate;
    private Player player;
    public bool InteractionBlocked { get; set; }
    public event ISwitcher.SwitchedEventHandler Switched;
    
    public bool SwitchedOn { 
        get => _itemsDeposited;
        set
        {
            _itemsDeposited = value;
            Switched?.Invoke(value);

            if (value)
            {
                SpriteWrapper.Play("3-on");
            }
            else
            {
                SpriteWrapper.Play("3-off");
            }
        }
    }
    private bool _itemsDeposited = false;

    public override void _Ready()
    {
        SpriteWrapper.Init(Sprite);
        
        balloonTemplate = GetNode<DialogueBalloon>(Names.NodePaths.DialogueBalloon);
        player = GetNode<Player>(Names.NodePaths.Player);
        
        SpriteWrapper.Play("3-off");
        
        CM.GetComponent<SaveEntity>().Setup("inventory_taker", (_) =>
        {
            SwitchedOn = true;
        });
        
        CM.GetComponent<SaveEntity>().OptionalInit(this);
        
    }

    public void OnInteracted()
    {
        StringName dialogueTitle;
        
        if (SwitchedOn)
        {
            dialogueTitle = "items_deposited";
        }
        else if (hasAllRequiredItems())
        {
            dialogueTitle = "has_all_items";
        }
        else
        {
            dialogueTitle = "missing_items";
        }
        
        balloonTemplate.PlayDialogue(dialogue, dialogueTitle);
        player.Controller.SwitchControl(balloonTemplate.Controller);
        
        DialogueManager.DialogueEnded += onDialogueEnded;
    }
    
    private bool hasAllRequiredItems()
    {
        foreach (StringName item in requiredItems)
        {
            bool hasItem = player.CM.GetComponent<PlayerInventory>().HasItem(item);
            if (!hasItem)
            {
                return false;
            }
        }
        return true;
    }
    
    private List<StringName> getMissingItems()
    {
        List<StringName> missing = new();
        foreach (StringName item in requiredItems)
        {
            bool hasItem = player.CM.GetComponent<PlayerInventory>().HasItem(item);
            if (!hasItem)
            {
                missing.Add(item);
            }
        }
        return missing;
    }
    
    private void onDialogueEnded(Resource dialogueResource)
    {
        DialogueManager.DialogueEnded -= onDialogueEnded;
        int choice = GlobalDialoguesState.Instance.Lab_InventoryTaker;
        
        if (choice == 1)
        {
            depositItems();
        } else if (choice == 2)
        {
            takeItemsBack();
        }

        GlobalDialoguesState.Instance.Lab_InventoryTaker = 0;
    }
    
    private void depositItems()
    {
        foreach (StringName item in requiredItems)
        {
            player.CM.GetComponent<PlayerInventory>().RemoveUniqueItem(item);
        }
        SwitchedOn = true;
        CM.GetComponent<SaveEntity>().ChangeState(true);
    }
    
    private void takeItemsBack()
    {
        foreach (StringName item in requiredItems)
        {
            player.CM.GetComponent<PlayerInventory>().AddUniqueItem(item);
        }
        SwitchedOn = false;
        CM.GetComponent<SaveEntity>().ChangeState(false);
    }
}
