using TENamespace.player_inventory;

namespace TerraEngineer.game.ui.inventory;

public static class InventoryChannel
{
    public delegate void ItemChosenEventHandler(InventoryItemData item);
    public static event ItemChosenEventHandler ItemChosen;

    public static void EmitItemChosen(InventoryItemData item)
    {
        ItemChosen?.Invoke(item);   
    }
}