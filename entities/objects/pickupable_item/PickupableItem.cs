using Godot;
using TENamespace.player_inventory;
using TENamespace.save_entity;
using TerraEngineer.entities.mobs;

namespace TerraEngineer.entities.objects;

[Tool]
public partial class PickupableItem : Entity
{
    [Export] private StringName itemName;
    [Export] private AtlasTexture ItemTexture {
        get => _itemTexture;
        set
        {
            Sprite.SpriteFrames.SetFrame(Names.Animations.Default, 0, value);
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
            Sprite.Visible = Collected;
        }
    }
    private bool _collected = false;

    public override void _Ready()
    {
        Sprite.SpriteFrames.SetFrame(Names.Animations.Default, 0, _itemTexture);
        CM.GetComponent<SaveEntity>().Setup("green_essence_exists", ((_) => Collected = true));
        CM.GetComponent<SaveEntity>().OptionalInit(this);
    } 
    
    private void onPlayerEntered(Node2D body)
    {
        if(Collected) return;
        
        Player player = (Player)body;
        player.CM.GetComponent<PlayerInventory>().AddItem(player, itemName);
        CM.GetComponent<SaveEntity>().ChangeState(false);
        Collected = true;
    }
}