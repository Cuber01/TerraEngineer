using Godot;
using System;
using TENamespace.advanced.terraform_gun;
using TerraEngineer.entities.objects;
using TerraEngineer.ui.player_hud;

public partial class EssenceSelectUI : Node2D, IConnectable<Player>
{
    // Has to be sorted by Biomes' enum id
    [Export] private AnimatedSprite2D[] essenceList;
    
    [Export] private AnimatedSprite2D decor;

    private TerraformGun terraformGun;
    private GunHandle gunHandle;
    
    int currentlySelected = 0;
    
    public void Connect(Player actor)
    {
        gunHandle = actor.CM.GetComponent<GunHandle>();
        terraformGun = gunHandle.CM.GetComponent<TerraformGun>();

        gunHandle.GunHandleChanged += onGunHandleChanged;
        terraformGun.EssenceChanged += onSelect;
        terraformGun.EssenceUnlocked += onUnlock;
    }

    public void Disconnect(Player actor)
    {
        gunHandle.GunHandleChanged -= onGunHandleChanged;
        terraformGun.EssenceChanged -= onSelect;
        terraformGun.EssenceUnlocked -= onUnlock;
    }

    private void onUnlock(Biomes biome)
    {
        essenceList[(int)biome].Animation = "selected";
    }
    
    private void onSelect(Biomes selected, Biomes unselected)
    {
        currentlySelected = (int)selected;
        essenceList[(int)selected].Animation = "selected";
        essenceList[(int)unselected].Animation = "unselected";
    }

    private void onGunHandleChanged(GunHandleType newSelected)
    {
        if (newSelected == GunHandleType.Terraforming)
        {
            essenceList[currentlySelected].Animation = "selected";
            decor.Animation = "selected";
        }
        else
        {
            decor.Animation= "unselected";
        }
    }
}
