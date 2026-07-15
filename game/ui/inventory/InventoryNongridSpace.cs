using Godot;
using System;
using TerraEngineer.game.ui.inventory;

public partial class InventoryNongridSpace : TextureButton
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
        updateState();
        FocusEntered += onFocusEntered;
    }

    private void onFocusEntered()
    {
        if (ItemData != null)
        {
            InventoryChannel.EmitItemChosen(ItemData);   
        }
    }

    private void updateState()
    {
        if (ItemData != null)
        {
            Disabled = false;
            FocusMode = FocusModeEnum.All;
        }
        else
        {
            Disabled = true;
            FocusMode = FocusModeEnum.None;
            
            Control bottom = GetNodeOrNull<Control>(FocusNeighborBottom);
            Control left = GetNodeOrNull<Control>(FocusNeighborLeft);
            Control right = GetNodeOrNull<Control>(FocusNeighborRight);
            Control top = GetNodeOrNull<Control>(FocusNeighborTop);
            if(bottom != null) bottom.FocusNeighborTop = FocusNeighborTop;
            if(left != null) left.FocusNeighborRight = FocusNeighborRight;
            if(right != null) right.FocusNeighborLeft = FocusNeighborLeft;
            if(top != null) top.FocusNeighborBottom = FocusNeighborBottom;
        }
    }
}
