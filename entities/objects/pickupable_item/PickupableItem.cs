using Godot;
using TENamespace.player_inventory;
using TENamespace.save_entity;
using TerraEngineer.entities.mobs;

namespace TerraEngineer.entities.objects;

[Tool]
public partial class PickupableItem : Entity
{
    [Export] private StringName itemName;
    [Export] private AtlasTexture Texture {
        get => itemTexture;
        set
        {
            Sprite.SpriteFrames.SetFrame(Names.Animations.Default, 0, value);
            itemTexture = value;
        } 
    }
    private AtlasTexture itemTexture;
    private bool collected = false;

    public override void _Ready()
    {
        Sprite.SpriteFrames.SetFrame(Names.Animations.Default, 0, itemTexture);
        CM.GetComponent<SaveEntity>().Setup("green_essence_exists", ((_) =>
        {
            collected = true;
            Sprite.Visible = false;
        }));
        CM.GetComponent<SaveEntity>().OptionalInit(this);
    } 
    
    private void onPlayerEntered(Node2D body)
    {
        if(collected) return;
        
        Player player = (Player)body;
        player.CM.GetComponent<PlayerInventory>().AddItem(player, itemName);
        CM.GetComponent<SaveEntity>().ChangeState(false);
        Sprite.Visible = false;
        collected = true;
    }
}