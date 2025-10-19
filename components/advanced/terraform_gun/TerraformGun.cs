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
    
    private List<TerraformableCaretaker> terraformablesAffected = new List<TerraformableCaretaker>();
    
    public void Shoot(Vector2 position, Vector2 direction, float rotationDegrees, bool mobParent)
    {
        CM.GetComponent<StarParticleBuilder>().Build(position, direction, selectedBiome, mobParent ? Actor : null);
        applyTerraform(); // Maybe on timer
    }

    public override void _PhysicsProcess(double delta)
    {
        // Change area2d position
    }

    private void applyTerraform()
    {
        foreach (TerraformableCaretaker obj in terraformablesAffected)
        {
            obj.Terraform(selectedBiome);
        }
    }

    // Remember to link it in editor!!
    private void onTerraformableEntered(Node2D body) => terraformablesAffected.Add(body as TerraformableCaretaker);
    private void onTerraformableExited(Node2D body) => terraformablesAffected.Remove(body as TerraformableCaretaker);
    
}