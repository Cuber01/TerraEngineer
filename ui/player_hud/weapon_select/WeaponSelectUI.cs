using Godot;
using System;
using System.Collections.Generic;
using TENamespace.advanced.main_gun_wrapper;
using TerraEngineer;
using TerraEngineer.ui.player_hud;

public partial class WeaponSelectUI : Node2D, IConnectable<Player>
{
    [Export] private Texture2D[] gunTextures;
    [Export] private AnimatedSprite2D decor;
    [Export] private Sprite2D icon;

    private GunHandle gunHandle;
    private PistolGunHandle normalGunHandle;
    
    public void Connect(Player player)
    {
        gunHandle = player.CM.GetComponent<GunHandle>();
        normalGunHandle = gunHandle.CM.GetComponent<PistolGunHandle>();
        
        gunHandle.GunHandleChanged += onGunHandleChanged;
        normalGunHandle.PistolGunChanged += onGunChanged;
    }

    public void Disconnect(Player player)
    {
        gunHandle.GunHandleChanged -= onGunHandleChanged;
        normalGunHandle.PistolGunChanged -= onGunChanged;
    }

    private void onGunChanged(PistolGuns newSelected)
    {
        icon.Texture = gunTextures[(int)newSelected];
    }
    
    private void onGunHandleChanged(GunHandleType newSelected)
    {
        if (newSelected == GunHandleType.Pistol)
        {
            decor.Animation = Names.Animations.Selected;
        }
        else
        {
            decor.Animation= Names.Animations.Unselected;
        }
    }
}
