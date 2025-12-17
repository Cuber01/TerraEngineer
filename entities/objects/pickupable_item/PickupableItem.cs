using Godot;
using TENamespace.player_inventory;
using TENamespace.save_entity;
using TerraEngineer.entities.mobs;

namespace TerraEngineer.entities.objects;

public partial class PickupableItem : Entity
{
    [Export] private StringName itemName;
    [Export] private AtlasTexture texture;

    private void onPlayerEntered(Node2D body)
    {
        Player player = (Player)body;
        player.CM.GetComponent<PlayerInventory>().AddItem(player, itemName);
        CM.GetComponent<SaveEntity>().ChangeState(false);
        Die();
    }
}