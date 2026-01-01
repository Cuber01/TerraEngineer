using System;
using System.Collections.Generic;
using Godot;
using TENamespace;
using TENamespace.advanced.terraform_gun;
using TerraEngineer;
using TerraEngineer.entities.objects;

namespace TENamespace.player_inventory;

public partial class PlayerInventory : Component
{
    private Dictionary<StringName, Item> fullItemList = new()
    {
        { "double_jump", new DoubleJumpItem() },
        { "blowtorch", new BlowtorchItem() },
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

    public void AddItem(Player actor, StringName name)
    {
        SaveData.SetValue(Names.SaveSections.PlayerInventory,name, true);
        fullItemList[name].Activate(actor);
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
        actor.Controller.AddAction(Names.Actions.Attack, 
            () => actor.CM.GetComponent<GunHandle>().Shoot(actor.GetShootDirection(), false), Names.Actions.GroupWeapon);
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

