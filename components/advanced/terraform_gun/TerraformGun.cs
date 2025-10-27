using System.Collections.Generic;
using Godot;
using TENamespace.basic.particle_builder;
using TerraEngineer.entities;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.objects;

namespace TENamespace.advanced.terraform_gun;

public partial class TerraformGun : AdvancedComponent, IGun
{
    public delegate void EssenceChangedEventHandler(Biomes selected, Biomes unselected);
    public event EssenceChangedEventHandler EssenceChanged;
    
    public delegate void EssenceUnlockedEventHandler(Biomes biome);
    public event EssenceUnlockedEventHandler EssenceUnlocked;
    
    [Export] private Biomes selectedBiome = Biomes.Forest;
    [Export] private Area2D areaAffected;
    
    // Every enum biome corresponds to it's index in this array
    private Biomes[] modes = [Biomes.Forest, Biomes.Mushroom, Biomes.Ice, Biomes.Desert];
    
    private List<TerraformableCaretaker> terraformablesAffected = new List<TerraformableCaretaker>();

    public void Shoot(Vector2 position, Vector2 direction, float rotationDegrees)
    {
        CM.GetComponent<StarParticleSpawner>()
                  .Start()
                  .SetBiome(selectedBiome)
                  .SetPosition(position)
                  .SetDirectionNormal(direction)
                  .Build();

        CM.GetComponent<StarParticleSpawner>().AddToGame();
        
        areaAffected.RotationDegrees = rotationDegrees;
        applyTerraform(); // Maybe on timer
    }

    public void ChangeWeapon(int index)
    {
        if (modes[index] != Biomes.Locked)
        {
            Biomes toSelect = (Biomes)index;
            EssenceChanged?.Invoke(toSelect, selectedBiome);
            selectedBiome = toSelect;
        }
    }

    public void ChangeToNextWeapon()
    {
        int i = (int)selectedBiome + 1;
        while (true)
        {
            if (i == modes.Length)
            {
                // Loop back
                i = 0;
            }
            
            if (modes[i] != Biomes.Locked)
            {
                Biomes toSelect = (Biomes)i;
                EssenceChanged?.Invoke(toSelect, selectedBiome);
                selectedBiome = (Biomes)i;
                break;
            }
            
            i++;
        }
    }

    public void LockOrUnlockMode(Biomes biome, bool unlock)
    {
        modes[(int)biome] = unlock ? biome : Biomes.Locked;
        if(unlock) EssenceUnlocked?.Invoke(biome);
    }
    
    private void applyTerraform()
    {
        foreach (TerraformableCaretaker obj in terraformablesAffected)
        {
            obj.Terraform(selectedBiome);
        }
    }

    private void onTerraformableEntered(Node2D body) => terraformablesAffected.Add(body as TerraformableCaretaker);
    private void onTerraformableExited(Node2D body) => terraformablesAffected.Remove(body as TerraformableCaretaker);
    
}