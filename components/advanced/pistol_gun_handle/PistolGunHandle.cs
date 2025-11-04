using System.Collections.Generic;
using Godot;
using TENamespace.advanced.blowtorch;
using TENamespace.advanced.shotgun;

namespace TENamespace.advanced.main_gun_wrapper;

// For non-terraforming guns

public enum PistolGuns
{
    Blowtorch,
    Shotgun,
}

public interface IPistolGun
{
    public void Shoot(Vector2 position, Vector2 direction, float rotationDegrees);
}

public partial class PistolGunHandle : AdvancedComponent, IGun
{
    public delegate void PistolGunChangedEventHandler(PistolGuns newSelected);
    public event PistolGunChangedEventHandler PistolGunChanged;
    
    private List<IPistolGun> guns = new List<IPistolGun>();
    private int selectedIndex;

    public override void _Ready()
    {
        guns.Add(CM.GetComponent<Blowtorch>());
        guns.Add(CM.GetComponent<Shotgun>());
    }
    
    public void Shoot(Vector2 position, Vector2 direction, float rotationDegrees)
        => guns[selectedIndex].Shoot(position, direction, rotationDegrees);

    public void ChangeWeapon(int index)
    {
        if (index < guns.Count)
        {
            selectedIndex = index;
            PistolGunChanged?.Invoke((PistolGuns)index);
        }
    }

    public void ChangeToNextWeapon()
    {
        int i = selectedIndex;
        i++;

        if (i == guns.Count)
        {
            // Loop back
            i = 0;
        }

        selectedIndex = i;
        PistolGunChanged?.Invoke((PistolGuns)selectedIndex);
    }

    public void UnlockGun(IPistolGun gun)
    {
        guns.Add(gun);
    }
}