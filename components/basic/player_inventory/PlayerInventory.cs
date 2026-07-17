using System.Collections.Generic;
using Godot;
using System;
using TENamespace.advanced.main_gun_wrapper;
using TENamespace.advanced.shotgun;
using TENamespace.advanced.terraform_gun;
using TENamespace.health;
using TerraEngineer;
using TerraEngineer.entities.objects;

namespace TENamespace.player_inventory;

public partial class PlayerInventory : Component
{
    private Dictionary<StringName, Item> fullItemList = new()
    {
        { "double_jump", new DoubleJumpItem() },
        { "blowtorch", new BlowtorchItem() },
        {"health_serum", new HealthSerumItem() },
        {"phase_teleporter", new PhaseTeleporterItem() },
        {"rifle", new RifleItem() },
        { "green_essence", new EssenceItem(Biomes.Forest) },
        { "blue_essence", new EssenceItem(Biomes.Ice) },
        { "orange_essence", new EssenceItem(Biomes.Desert) },
        { "purple_essence", new EssenceItem(Biomes.Mushroom) },
        { "ice_crystal", new IceCrystalItem() },
        { "gunpowder", new KeyItem() },
        { "vine", new KeyItem() },
        { "mushroom_cap", new KeyItem() },
    };

    private List<Item> inventoryItems = new();
    
    private Player playerActor;
    
    public override void Init(Node2D actor)
    {
        base.Init(actor);
        if (actor is Player player)
        {
            playerActor = player;
        }
        else
        {
            throw new Exception("Interacter component requires Player actor.");
        }
    }
    
    public void ActivateItems()
    {
        List<StringName> itemsLastRead = SaveData.ReadInventory();
        foreach (StringName item in itemsLastRead)
        {
            fullItemList[item].Activate(playerActor);
            inventoryItems.Add(fullItemList[item]);
        }
    }

    public bool TryAddUniqueItem(StringName name)
    {
        Variant hasItem = SaveData.ReadValue(Names.SaveSections.PlayerInventory, name);
        if (!MathT.IsTrue(hasItem))
        {
            AddUniqueItem(name);
            return true;
        }
        return false;
    }

    public void AddUniqueItem(StringName name)
    {
        SaveData.SetAddValue(Names.SaveSections.PlayerInventory,name, true);
        inventoryItems.Add(fullItemList[name]);
        
        // Special handling for essences: auto-equip when newly acquired
        if (fullItemList[name] is EssenceItem essenceItem)
        {
            essenceItem.ActivateAndEquip(playerActor);
        }
        else
        {
            fullItemList[name].Activate(playerActor);
        }
    }

    public bool HasItem(StringName name)
    {
        return MathT.IsTrue(
            SaveData.ReadValue(Names.SaveSections.PlayerInventory,
                name));
    }

    public void RemoveUniqueItem(StringName name)
    {
        Item item = fullItemList[name];
        item.Deactivate(playerActor);
        SaveData.SetAddValue(Names.SaveSections.PlayerInventory, name, false);
        inventoryItems.Remove(item);
    }

    public void AddGenericItem(Player actor, StringName name, int amount)
    {
        inventoryItems.Add(fullItemList[name]);
        int currentAmount = (int)SaveData.ReadValue(Names.SaveSections.PlayerInventory, name);
        SaveData.SetAddValue(Names.SaveSections.PlayerInventory,name, currentAmount + amount);
        fullItemList[name].Activate(actor);
    }
}

public enum ItemType
{
    Unique, // You can have only one
    Generic // You can have many (coins, health etc.)
}

public interface Item
{
    public void Activate(Player actor);
    public void Deactivate(Player actor);
}

public class DoubleJumpItem : Item
{
    public void Activate(Player actor)
    {
        actor.CM.GetComponent<Jump>().MaxJumps = 2;
    }
    public void Deactivate(Player actor)
    {
        throw new NotImplementedException();
    }
}

public class BlowtorchItem : Item
{
    public void Activate(Player actor)
    {
        actor.Controller.AddAction(Names.Actions.Attack, 
            () => actor.CM.GetComponent<GunHandle>().Shoot(actor.GetShootDirection(), false), Names.Actions.GroupWeapon);
    }
    public void Deactivate(Player actor)
    {
        throw new NotImplementedException();
    }
}

public class RifleItem : Item
{
    public void Activate(Player actor)
    {
        actor.CM.GetComponent<GunHandle>().CM.GetComponent<PistolGunHandle>().UnlockGun<Rifle>();
    }
    public void Deactivate(Player actor)
    {
        throw new NotImplementedException();
    }
}

public class PhaseTeleporterItem : Item
{
    public void Activate(Player actor)
    {
        actor.PhasingAllowed = true;
    }
    public void Deactivate(Player actor)
    {
    }
}

public class HealthSerumItem : Item
{
    public void Activate(Player actor)
    {
        actor.CM.GetComponent<Health>().MaxHealth += 1;
        actor.CM.GetComponent<Health>().FullHeal();
    }
    public void Deactivate(Player actor)
    {
        throw new NotImplementedException();
    }
}

public class IceCrystalItem : Item
{
    public void Activate(Player actor)
    {

    }
    public void Deactivate(Player actor)
    {
        
    }
}

public class KeyItem : Item
{
    public void Activate(Player actor)
    {

    }
    
    public void Deactivate(Player actor)
    {
        
    }
}


public class EssenceItem(Biomes biome) : Item
{
    public void Activate(Player actor)
    {
        GunHandle gunHandle = actor.CM.GetComponent<GunHandle>();
        gunHandle.CM.GetComponent<TerraformGun>().LockOrUnlockMode(biome, true);
    }
    
    public void Deactivate(Player actor)
    {
        GunHandle gunHandle = actor.CM.GetComponent<GunHandle>();
        gunHandle.CM.GetComponent<TerraformGun>().LockOrUnlockMode(biome, false);
    }

    public void ActivateAndEquip(Player actor)
    {
        Activate(actor);
        GunHandle gunHandle = actor.CM.GetComponent<GunHandle>();
        if(gunHandle.SelectedGun == GunHandleType.Pistol)
            gunHandle.ChangeGunHandle();
    }
}

