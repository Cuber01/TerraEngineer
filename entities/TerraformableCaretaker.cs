using Godot;
using System;
using System.Collections.Generic;
using TerraEngineer.entities;
using TerraEngineer.entities.objects;

public partial class TerraformableCaretaker : Node2D
{
    [Export] private Biomes currentBiome;
    private Dictionary<Biomes, Terraformable> entityVersions = new Dictionary<Biomes, Terraformable>();
    
    public override void _Ready()
    {
        foreach (var node in GetChildren())
        {
            var entity = (Terraformable)node;
            entity.Disable();
            entityVersions.Add(entity.MyBiome, entity);
        }
        
        Terraform(currentBiome);
    }

    public void Terraform(Biomes biome)
    {
        entityVersions[currentBiome].Disable();
        entityVersions[biome].Enable();
        currentBiome = biome;
    }
}
