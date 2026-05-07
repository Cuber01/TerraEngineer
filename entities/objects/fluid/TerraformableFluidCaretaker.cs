namespace TerraEngineer.entities.objects.fluid;

// Wrapper for fluid to be terraformable since we want to use different tech than the usual setup for objects
public partial class TerraformableFluidCaretaker : TerraformableCaretaker
{
    private Fluid fluid;

    public override void _Ready()
    {
        fluid = GetParent() as Fluid;
    }

    public override void _PhysicsProcess(double delta) { }
    
    public override void Terraform(Biomes biome)
    {
        fluid.Terraform(biome);
    }
}