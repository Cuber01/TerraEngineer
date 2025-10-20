using Godot;
using System;
using System.Collections.Generic;
using TerraEngineer.entities;
using TerraEngineer.entities.objects;

public partial class TerraformableCaretaker : Node2D
{
    [Export] private Biomes currentBiome;
    [Export] private Node2D versions;

    private bool init = false;
    
    private Dictionary<Biomes, Terraformable> entityVersions = new Dictionary<Biomes, Terraformable>();
    
    public override void _Ready()
    {
        foreach (var node in versions.GetChildren())
        {
            var entity = (Terraformable)node;
            entity.Disable();
            entityVersions.Add(entity.MyBiome, entity);
        }
        
        Terraform(currentBiome);
        init = true;
    }

    public void Terraform(Biomes biome)
    {
        if (currentBiome == biome && init) return;
        
        entityVersions[currentBiome].Disable();
        entityVersions[biome].Enable();
        currentBiome = biome;
    }
}
