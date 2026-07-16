using Godot;
using System;
using System.Collections.Generic;
using TENamespace.player_inventory;
using TerraEngineer.game;
using TerraEngineer.ui.player_hud;

namespace TerraEngineer.game.ui.inventory;

public partial class InventoryScreenStarter : Node2D, IConnectable<Player>
{
    [Export] private PackedScene inventoryScene;
    private Controller controller;
    private Player player;
    private Control instantiatedInventory;

    private static readonly Dictionary<string, string> ItemToNodeName = new()
    {
        { "double_jump", "Wings" },
        { "blowtorch", "Blowtorch" },
        { "phase_teleporter", "PhaseShift" },
        { "rifle", "Rifle" },
        { "green_essence", "Green" },
        { "blue_essence", "Blue" },
        { "purple_essence", "Purple" },
        { "grenade_launcher", "GrenadeLauncher"},
        { "dash", "Dash"},
        { "health_serum", "HealthContainerAmount" },  // Special case: not a button
    };

    private static readonly Dictionary<string, string> ItemToDataPaths = new()
    {
        { "double_jump", "res://game/ui/inventory/items/double_jump_item.tres" },
        { "blowtorch", "res://game/ui/inventory/items/blowtorch_item.tres" },
        { "health_serum", "res://game/ui/inventory/items/health_serum_item.tres" },
        { "phase_teleporter", "res://game/ui/inventory/items/phase_teleporter_item.tres" },
        { "rifle", "res://game/ui/inventory/items/rifle_item.tres" },
        { "green_essence", "res://game/ui/inventory/items/green_essence_item.tres" },
        { "blue_essence", "res://game/ui/inventory/items/blue_essence_item.tres" },
        { "purple_essence", "res://game/ui/inventory/items/purple_essence_item.tres" },
        { "ice_crystal", "res://game/ui/inventory/items/ice_crystal.tres"},
        { "vine", "res://game/ui/inventory/items/vine.tres"},
        { "gunpowder", "res://game/ui/inventory/items/gunpowder.tres"},
        { "mushroom_cap", "res://game/ui/inventory/items/mushroom_cap.tres"}
    };

    public override void _Ready()
    {
        controller = new Controller();
        controller.AddAction(Names.Actions.Quit, close);
        controller.AddAction(Names.Actions.OpenInventory, close);
    }

    public override void _Process(double delta)
    {
        controller.Update();
    }

    private void open(Controller oldController)
    {
        oldController.SwitchControl(controller);
        
        instantiatedInventory = (Control)inventoryScene.Instantiate();
        AddChild(instantiatedInventory);

        PopulateInventory(instantiatedInventory);
        
        Callable.From(SetFocusToBlowtorch).CallDeferred();
        
        GetTree().Paused = true;
    }
    
    private void close()
    {
        player.InvokeCloseInventory();
        controller.GiveBackControl(false);

        if (instantiatedInventory != null)
        {
           instantiatedInventory.CallDeferred(Node.MethodName.QueueFree);
           instantiatedInventory = null;
        }
        
        GetTree().Paused = false;
    }

    private void PopulateInventory(Control inventoryScreen)
    {
        List<StringName> inventoryItems = SaveData.ReadInventory();
        List<string> remainingItems = new();
        
        // First pass: place mapped items on specific nodes
        foreach (StringName itemName in inventoryItems)
        {
            string itemNameStr = itemName.ToString();
            bool itemPlaced = false;
            
            if (ItemToNodeName.TryGetValue(itemNameStr, out string nodeName))
            {
                var node = inventoryScreen.FindChild(nodeName);
                if (node is InventoryNongridSpace nongridSpace)
                {
                    if (ItemToDataPaths.TryGetValue(itemNameStr, out string resourcePath))
                    {
                        var resourceFile = (InventoryItemData)ResourceLoader.Load(resourcePath);
                        nongridSpace.ItemData = resourceFile;
                        itemPlaced = true;
                    }
                }
            }

            if (!itemPlaced)
            {
                remainingItems.Add(itemNameStr);
            }
        }
        
        // Second pass: place remaining items in grid from left to right
        var gridContainer = inventoryScreen.FindChild("GridContainer") as GridContainer;

        int gridIndex = 0;
        int itemIndex = 0;
        
        foreach (var child in gridContainer.GetChildren())
        {
            if (itemIndex >= remainingItems.Count) break;
            
            var gridSpace = child as InventoryGridSpace;
            if (gridSpace != null)
            {
                string itemName = remainingItems[itemIndex];
                if (ItemToDataPaths.TryGetValue(itemName, out string resourcePath))
                {
                    var resourceFile = (InventoryItemData)ResourceLoader.Load(resourcePath);
                    gridSpace.ItemData = resourceFile;
                    itemIndex++;
                }
            }
        }
        
        // Third pass: Update state of all nongrid spaces 
        // This has to be done here because else all nodes would first execute as disabled
        foreach(var pair in ItemToNodeName)
        {
            var space = inventoryScreen.GetNodeOrNull<InventoryNongridSpace>(pair.Value);
            
            if(space != null)
            {
                space.UpdateState();
            }
        }
        
    }

    private void SetFocusToBlowtorch()
    {
        if (instantiatedInventory.FindChild("Blowtorch") is Control flamethrower)
        {
            flamethrower.GrabFocus();
        }
    }

    public void Connect(Player actor)
    {
        actor.OpenInventory += open;
        this.player = actor;
    }

    public void Disconnect(Player actor)
    {
        actor.OpenInventory -= open;
    }
}
