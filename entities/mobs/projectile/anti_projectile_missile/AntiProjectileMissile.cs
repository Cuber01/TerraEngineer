using Godot;
using System;
using TENamespace.basic.orbit;
using TerraEngineer.entities.projectiles;

public partial class AntiProjectileMissile : Projectile
{
	public override void _PhysicsProcess(double delta)
	{
		CM.UpdateComponents((float)delta);
        
		CM.GetComponent<Orbit>().OrbitAround(Creator.GlobalPosition, (float)delta);
        
		HandleMove();
	}
}
