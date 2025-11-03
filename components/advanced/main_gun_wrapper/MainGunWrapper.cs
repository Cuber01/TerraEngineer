using System.Collections.Generic;
using Godot;
using TENamespace.advanced.blowtorch;
using TENamespace.advanced.shotgun;

namespace TENamespace.advanced.main_gun_wrapper;

// For non-terraforming guns

public enum NormalGuns
{
    Blowtorch,
    Shotgun,
}

public interface IMainGun
{
    public void Shoot(Vector2 position, Vector2 direction, float rotationDegrees);
}

public partial class MainGunWrapper : AdvancedComponent, IGun
{
    public delegate void MainGunChangedEventHandler(NormalGuns newSelected);
    public event MainGunChangedEventHandler NormalGunChanged;
    
    private List<IMainGun> guns = new List<IMainGun>();
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
            NormalGunChanged?.Invoke((NormalGuns)index);
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
        NormalGunChanged?.Invoke((NormalGuns)selectedIndex);
    }

    public void UnlockGun(IMainGun gun)
    {
        guns.Add(gun);
    }
}