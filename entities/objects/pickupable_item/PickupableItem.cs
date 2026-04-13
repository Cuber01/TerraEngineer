using Godot;
using TENamespace.player_inventory;
using TENamespace.save_entity;
using TENamespace.ui.dialogue_box;
using TerraEngineer.entities.mobs;

namespace TerraEngineer.entities.objects;

[Tool]
public partial class PickupableItem : Entity
{
    [Export] private StringName itemName;
    [Export] private StringName itemCollectedTag;
    [Export] private ItemType itemType = ItemType.Unique;
    [Export] private int itemAmount = 1;
    [Export] private Resource dialogueDescription;
    [Export] private AtlasTexture ItemTexture {
        get => _itemTexture;
        set
        {
            if (Sprite != null && SpriteWrapper is { Initialized: false })
            {
                SpriteWrapper.Init(Sprite);
                SpriteWrapper.SetTexture(value);
            }
                
            _itemTexture = value;
        } 
    }
    private AtlasTexture _itemTexture;

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
    
    private DialogueBalloon balloonTemplate;
    private Player player;

    public override void _Ready()
    {
        player = GetNode<Player>(Names.NodePaths.Player);
        balloonTemplate = GetNode<DialogueBalloon>(Names.NodePaths.DialogueBalloon);
     
        //Sprite.SpriteFrames.SetFrame(Names.Animations.Default, 0, (Texture2D)_itemTexture.Duplicate());
        CM.GetComponent<SaveEntity>().Setup(itemCollectedTag, ((_) => Collected = true));
        CM.GetComponent<SaveEntity>().OptionalInit(this);
    } 
    
    private void onPlayerEntered(Node2D body)
    {
        if(Collected) return;

        if (itemType == ItemType.Unique)
        {
            player.CM.GetComponent<PlayerInventory>().AddUniqueItem(player, itemName);
        } else if (itemType == ItemType.Generic)
        {
            player.CM.GetComponent<PlayerInventory>().AddGenericItem(player, itemName, itemAmount);
        }
        CM.GetComponent<SaveEntity>().ChangeState(true);
        Collected = true;
        
        balloonTemplate.PlayDialogue(dialogueDescription, Names.Other.Start);
        player.Controller.SwitchControl(balloonTemplate.Controller);
    }
    
}