using Godot;
using System;
using TENamespace.advanced.terraform_gun;
using TerraEngineer.entities.objects;
using TerraEngineer.ui.player_hud;

public partial class EssenceSelect : Node2D, IConnectable<Player>
{
    // Has to be sorted by Biomes' enum id
    [Export] private AnimatedSprite2D[] essenceList;
    
    [Export] private AnimatedSprite2D decor;

    private TerraformGun terraformGun;
    
    public void Connect(Player actor)
    {
        // TODO turn this into a CM method
        terraformGun = actor.CM.GetComponent<GunHandle>().CM.GetComponent<TerraformGun>();

        terraformGun.EssenceChanged += onSelect;
        terraformGun.EssenceUnlocked += onUnlock;
    }

    public void Disconnect(Player actor)
    {
        terraformGun.EssenceChanged -= onSelect;
        terraformGun.EssenceUnlocked -= onUnlock;
    }

    private void onUnlock(Biomes biome)
    {
        essenceList[(int)biome].Animation = "selected";
    }
    
    private void onSelect(Biomes selected, Biomes unselected)
    {
        essenceList[(int)selected].Animation = "selected";
        essenceList[(int)unselected].Animation = "unselected";
    }
}
