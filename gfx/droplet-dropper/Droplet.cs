using Godot;
using System;
using TerraEngineer.entities.mobs;

public partial class Droplet : Entity
{
    public override void FellIntoFluid()
    {
        Die(); // Possible call for on area exited? watch out
    }

    public override void _PhysicsProcess(double delta)
    {
        CM.UpdateComponents((float)delta);
        HandleMove();
    }
}
