using Godot;
using System;
using System.Collections.Generic;
using TENamespace.advanced.main_gun_wrapper;
using TerraEngineer.ui.player_hud;

public partial class WeaponSelectUI : Node2D, IConnectable<Player>
{
    [Export] private Texture2D[] gunTextures;
    [Export] private AnimatedSprite2D decor;
    [Export] private Sprite2D icon;

    private GunHandle gunHandle;
    private MainGunWrapper normalGunWrapper;
    
    public void Connect(Player player)
    {
        gunHandle = player.CM.GetComponent<GunHandle>();
        normalGunWrapper = gunHandle.CM.GetComponent<MainGunWrapper>();
        
        gunHandle.GunHandleChanged += onGunHandleChanged;
        normalGunWrapper.NormalGunChanged += onGunChanged;
    }

    public void Disconnect(Player player)
    {
        gunHandle.GunHandleChanged -= onGunHandleChanged;
        normalGunWrapper.NormalGunChanged -= onGunChanged;
    }

    private void onGunChanged(NormalGuns newSelected)
    {
        icon.Texture = gunTextures[(int)newSelected];
    }
    
    private void onGunHandleChanged(GunHandleType newSelected)
    {
        if (newSelected == GunHandleType.Normal)
        {
            decor.Animation = "selected";
        }
        else
        {
            decor.Animation= "unselected";
        }
    }
}
