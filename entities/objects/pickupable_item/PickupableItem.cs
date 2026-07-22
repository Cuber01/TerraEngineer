using DialogueManagerRuntime;
using Godot;
using TENamespace.player_inventory;
using TENamespace.save_entity;
using TENamespace.ui.dialogue_box;
using TerraEngineer.entities.mobs;
using TerraEngineer.game;
using TerraEngineer.game.ui;

namespace TerraEngineer.entities.objects;

[Tool]
public partial class PickupableItem : Entity, IInteractable
{
    [Export] private StringName itemName;
    [Export] private StringName itemCollectedTag;
    [Export] private ItemType itemType = ItemType.Unique;
    [Export] private int itemAmount = 1;
    [Export] private Resource dialogueDescription;
    [Export] private Resource alreadyHaveItemDialogue;
    [Export] private AtlasTexture ItemTexture {
        get => _itemTexture;
        set
        {
            if(Sprite != null && value != null)
                ((Sprite2D)Sprite).Texture = (Texture2D)value.Duplicate(true); 
            
            _itemTexture = value;
        } 
    }
    private AtlasTexture _itemTexture;
    
    [Export] private bool canBeRecollected = false;

    private bool Collected
    {
        get => _collected;
        set
        {
            _collected = value;
            Sprite.Visible = !Collected;
        }
    }
    private bool _collected = false;
    
    public bool InteractionBlocked { get; set; }
    
    private DialogueBalloon balloonTemplate;
    private Player player;

    public override void _Ready()
    {
        player = GetNode<Player>(Names.NodePaths.Player);
        balloonTemplate = GetNode<DialogueBalloon>(Names.NodePaths.DialogueBalloon);
        
        //MetSysApi.RegisterStorableObjectWithMarker(this, () => {}, )
     
        ((Sprite2D)Sprite).Texture = (Texture2D)_itemTexture.Duplicate(true); 

        if (!canBeRecollected)
        {
            InteractionBlocked = true;
        }
     
        #if DEBUG
        if(Engine.IsEditorHint())
            return;
        #endif
        
        CM.GetComponent<SaveEntity>().Setup(itemCollectedTag, ((_) => Collected = true));
        CM.GetComponent<SaveEntity>().OptionalInit(this);
    } 
    
    private void onPlayerEntered(Node2D body)
    {
        if(!canBeRecollected)
        {
            handleSingleCollect();
        }
    }

    private void handleSingleCollect()
    {
        if(Collected) return;
        
        CM.GetComponent<SaveEntity>().ChangeState(true);
        Collected = true;
        
        Collected = tryGetItem();
    }

    private bool tryGetItem()
    {
        bool success = false;
        
        if (itemType == ItemType.Unique)
        {
            success = player.CM.GetComponent<PlayerInventory>().TryAddUniqueItem(itemName);
        } else if (itemType == ItemType.Generic)
        {
            player.CM.GetComponent<PlayerInventory>().AddGenericItem(player, itemName, itemAmount);
            success = true;
        }

        if (success)
        {
            balloonTemplate.PlayDialogue(dialogueDescription, Names.Other.Start);
        }
        else
        {
            balloonTemplate.PlayDialogue(alreadyHaveItemDialogue, Names.Other.Start);
        }
        InputStackManager.Push(balloonTemplate.InputContext);
        return success;
    }

    public void OnInteracted() => getItem();
}