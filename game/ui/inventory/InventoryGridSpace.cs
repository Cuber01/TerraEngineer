using Godot;
using System;
using TerraEngineer.game.ui.inventory;
using TerraEngineer.ui.player_hud;

public partial class InventoryGridSpace : TextureRect
{
    [Export] public InventoryItemData ItemData
    {
        set
        {
            _itemData = value;
            updateIcon();
        }
        private get => _itemData;
    }
    private InventoryItemData _itemData;
    
    [Export] private Sprite2D selectedSprite;
    [Export] private Sprite2D itemIcon;
    
    public override void _Ready()
    {
        FocusEntered += onFocusEntered;
        FocusExited += onFocusExited;
    }
    
    private void updateIcon()
    {
        itemIcon.Texture = _itemData.Icon;
    }    
    
    private void onFocusEntered()
    {
        selectedSprite.Visible = true;
        if (ItemData != null)
        {
            InventoryChannel.EmitItemChosen(ItemData);    
        }
        
    }

    private void onFocusExited()
    {
        selectedSprite.Visible = false;
    }
}
