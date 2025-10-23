using System.Collections.Generic;
using Godot;
using TENamespace.basic.particle_builder;
using TerraEngineer.entities;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.objects;

namespace TENamespace.advanced.terraform_gun;

public partial class TerraformGun : AdvancedComponent, IGun
{
    [Export] private Biomes selectedBiome = Biomes.Forest;
    [Export] private Area2D areaAffected;
    
    // Every enum biome corresponds to it's index in this array
    private Biomes[] modes = [Biomes.Forest, Biomes.Ice, Biomes.Mushroom, Biomes.Desert];
    
    private List<TerraformableCaretaker> terraformablesAffected = new List<TerraformableCaretaker>();
    
    public void Shoot(Vector2 position, Vector2 direction, float rotationDegrees)
    {
        Node2D instance = CM.GetComponent<StarParticleSpawner>().Build(position, direction, selectedBiome);
        CM.GetComponent<StarParticleSpawner>().AddToGame(instance);
        areaAffected.RotationDegrees = rotationDegrees;
        applyTerraform(); // Maybe on timer
    }

    public void ChangeWeapon(int index)
    {
        if (modes[index] != Biomes.Locked)
        {
            selectedBiome = (Biomes)index;
            GD.Print(selectedBiome);
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
                selectedBiome = (Biomes)i;
                GD.Print(selectedBiome);
                break;
            }
            
            i++;
        }
    }

    public void LockOrUnlockMode(Biomes biome, bool unlock)
    {
        modes[(int)biome] = unlock ? biome : Biomes.Locked;
    }

    private int i = 0;
    private void applyTerraform()
    {
        foreach (TerraformableCaretaker obj in terraformablesAffected)
        {
            i++;
            obj.Terraform(selectedBiome);
        }
    }

    private void onTerraformableEntered(Node2D body) => terraformablesAffected.Add(body as TerraformableCaretaker);
    private void onTerraformableExited(Node2D body) => terraformablesAffected.Remove(body as TerraformableCaretaker);
    
}