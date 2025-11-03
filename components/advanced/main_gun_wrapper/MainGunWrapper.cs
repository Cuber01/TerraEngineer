using System.Collections.Generic;
using Godot;
using TENamespace.advanced.blowtorch;
using TENamespace.advanced.shotgun;

namespace TENamespace.advanced.main_gun_wrapper;

// For non-terraforming guns

public interface IMainGun
{
    public void Shoot(Vector2 position, Vector2 direction, float rotationDegrees);
}

public partial class MainGunWrapper : AdvancedComponent, IGun
{
    private List<IMainGun> guns = new List<IMainGun>();
    private int selectedIndex;

    public override void _Ready()
    {
        guns.Add(CM.GetComponent<Shotgun>());
        guns.Add(CM.GetComponent<Blowtorch>());
    }
    
    public void Shoot(Vector2 position, Vector2 direction, float rotationDegrees)
        => guns[selectedIndex].Shoot(position, direction, rotationDegrees);

    public void ChangeWeapon(int index)
    {
        if (index < guns.Count)
        {
            selectedIndex = index;
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
    }

    public void UnlockGun(IMainGun gun)
    {
        guns.Add(gun);
    }
}