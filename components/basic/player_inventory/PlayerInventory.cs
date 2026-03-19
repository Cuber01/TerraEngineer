using System.Collections.Generic;
using Godot;
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
        { "green_essence", new EssenceItem(Biomes.Forest) },
        { "blue_essence", new EssenceItem(Biomes.Ice) },
        { "orange_essence", new EssenceItem(Biomes.Desert) },
        { "purple_essence", new EssenceItem(Biomes.Mushroom) },
    };

    private List<StringName> itemsLastRead = new();
    
    public void ActivateItems(Player actor)
    {
        itemsLastRead = SaveData.ReadInventory();
        foreach (StringName item in itemsLastRead)
        {
            fullItemList[item].Activate(actor);
        }
    }

    public void AddUniqueItem(Player actor, StringName name)
    {
        SaveData.SetAddValue(Names.SaveSections.PlayerInventory,name, true);
        fullItemList[name].Activate(actor);
    }

    public void AddGenericItem(Player actor, StringName name, int amount)
    {
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
        actor.Controller.AddAction(Names.Actions.Attack, 
            () => actor.CM.GetComponent<GunHandle>().Shoot(actor.GetShootDirection(), false), Names.Actions.GroupWeapon);
    }
}


public class HealthSerumItem : Item
{
    public void Activate(Player actor)
    {
        actor.CM.GetComponent<Health>().MaxHealth += 1;
        actor.CM.GetComponent<Health>().FullHeal();
    }
}


public class EssenceItem(Biomes biome) : Item
{
    public void Activate(Player actor)
    {
        GunHandle gunHandle = actor.CM.GetComponent<GunHandle>();
        
        gunHandle.CM.GetComponent<TerraformGun>().LockOrUnlockMode(biome, true);
        if(gunHandle.SelectedGun == GunHandleType.Pistol)
            gunHandle.ChangeGunHandle();
    }
}

