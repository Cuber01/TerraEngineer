using Godot;

namespace TerraEngineer.game.ui.inventory;

public partial class ItemDescriptionBox : Node2D
{
    [Export] private RichTextLabel nameLabel;
    [Export] private RichTextLabel descLabel;
    
    public override void _Ready()
    {
        InventoryChannel.ItemChosen += DisplayItem;
    }

    public void DisplayItem(InventoryItemData item)
    {
        nameLabel.Text = item.ItemName;
        descLabel.Text = item.Description;
    }
}