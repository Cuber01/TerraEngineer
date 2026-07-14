using Godot;
using System;

public partial class InventoryNeighborsRerouter : TextureButton
{
    [Export] public InventoryItemData ItemData
    {
        set
        {
            _itemData = value;
            updateState();
        }
        private get => _itemData;
    }
    private InventoryItemData _itemData;

    public override void _Ready()
    {
        base._Ready();
    }

    private void updateState()
    {
        if (ItemData != null)
        {
            Disabled = false;
        }
        else
        {
            Disabled = true;
        }
    }
}
