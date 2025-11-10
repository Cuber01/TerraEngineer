using System;
using System.Collections.Generic;
using Godot;
using TENamespace;
using TerraEngineer;

namespace TENamespace.player_inventory;

public partial class PlayerInventory : Component
{
    private Dictionary<StringName, Type> fullItemList = new()
    {
        { "double_jump", typeof(DoubleJumpItem) },
        { "blowtorch", typeof(BlowtorchItem) }
    };

    private List<StringName> itemsLastRead = new();
    
    public void ActivateItems(Player actor)
    {
        itemsLastRead = SaveData.ReadInventory();
        foreach (StringName item in itemsLastRead)
        {
            Type type = fullItemList[item];
            Item instance = (Item)Activator.CreateInstance(type);
            instance!.Activate(actor);
        }
    }

    public void AddItem(Player actor, StringName name)
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

public class BlowtorchItem : Item
{
    public void Activate(Player actor)
    {
        actor.controller.AddAction(Names.Actions.Attack, 
            () => actor.CM.GetComponent<GunHandle>().Shoot(actor.GetShootDirection(), false), Names.Actions.GroupWeapon);
    }
}

