using System;
using System.Collections.Generic;
using TerraEngineer;

namespace TENamespace.player_inventory;

public partial class PlayerInventory : Component
{
    private Dictionary<string, Type> fullItemList = new()
    {
        { "double_jump", typeof(DoubleJumpItem) }
    };

    private List<string> itemsLastRead = new();
    
    public void ActivateItems(Player actor)
    {
        itemsLastRead = SaveData.ReadInventory();
        foreach (string item in itemsLastRead)
        {
            Type type = fullItemList[item];
            Item instance = (Item)Activator.CreateInstance(type);
            instance!.Activate(actor);
        }
    }

    public void AddItem(Player actor, string name)
    {
        SaveData.SetValue(Names.SaveSections.PlayerInventory,name, true);
        Type type = fullItemList[name];
        Item instance = (Item)Activator.CreateInstance(type);
        instance!.Activate(actor);
    }
}

public interface Item
{
    public void Activate(Player actor);
    // public void Deactivate();
}

public class DoubleJumpItem : Item
{
    public void Activate(Player actor)
    {
        actor.CM.GetComponent<Jump>().MaxJumps = 2;
    }
}

