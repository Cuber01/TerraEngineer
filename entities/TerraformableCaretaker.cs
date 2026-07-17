using Godot;
using System.Collections.Generic;
using TerraEngineer.entities;
using TerraEngineer.entities.objects;

public partial class TerraformableCaretaker : Node2D
{
    [Export] private Biomes currentBiome;
    [Export] private Node2D versions;

    public delegate void TerraformedEventHandler(Biomes biome);
    public event TerraformedEventHandler Terraformed;
    
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

        if (GetParent() is TerraformableCaretaker myCaretaker)
        {
            myCaretaker.Terraformed += Terraform;
        }
        
        Terraform(currentBiome);
        init = true;
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("f10"))
        {
            var nextBiome = (Biomes)(((int)currentBiome + 1) % System.Enum.GetValues(typeof(Biomes)).Length);
            Terraform(nextBiome);
        }
    }

    public virtual void Terraform(Biomes biome)
    {
        if (currentBiome == biome && init) return;
        if(!entityVersions.ContainsKey(biome)) return; // This shouldn't happen in the full game theoretically
        
        entityVersions[currentBiome].Disable();
        entityVersions[biome].Enable();
        currentBiome = biome;
        Terraformed?.Invoke(biome);
    }
}
