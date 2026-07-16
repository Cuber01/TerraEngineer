using Godot;
using System.Collections.Generic;
using TerraEngineer.entities;
using TerraEngineer.entities.objects;

public partial class TerraformableCaretaker : CharacterBody2D
{
    [Export] private Biomes currentBiome;
    [Export] private Node2D versions;

    private bool init = false;
    
    private Dictionary<Biomes, ITerraformable> entityVersions = new Dictionary<Biomes, ITerraformable>();
    
    public override void _Ready()
    {
        foreach (var node in versions.GetChildren())
        {
            var entity = (ITerraformable)node;
            entity.Disable();
            entity.Setup(this);
            entityVersions.Add(entity.MyBiome, entity);
        }
        
        Terraform(currentBiome);
        init = true;
    }

    public virtual void Terraform(Biomes biome)
    {
        if (currentBiome == biome && init) return;
        if(!entityVersions.ContainsKey(biome)) return; // This shouldn't happen in the full game theoretically
        
        entityVersions[currentBiome].Disable();
        entityVersions[biome].Enable();
        currentBiome = biome;
    }
}
