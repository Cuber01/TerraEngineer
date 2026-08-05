using Godot;
using System;
using TENamespace.basic.orbit;
using TerraEngineer.entities.projectiles;

public partial class AntiProjectileMissile : Projectile
{
	private Node2D target = null;
	private bool idle = true;
	
	public override void _PhysicsProcess(double delta)
	{
		CM.UpdateComponents((float)delta);

		if (idle)
		{
			updateIdle((float)delta);	
		}
		else
		{
			updateFollow((float)delta);
		}
		
		HandleMove();
	}

	private void updateIdle(float delta)
	{
		target = CM.GetComponent<FindClosestNode>().GetClosest();
		if(target != null)
		{
			idle = false;
			return;
		}
	
		CM.GetComponent<Orbit>().OrbitAround(Creator.GlobalPosition, delta);
	}

	private void updateFollow(float delta)
	{
		if (target == null || !IsInstanceValid(target))
		{
			idle = true;
			return;
		}
		
		CM.GetComponent<Orbit>().FlyToPoint(target.GlobalPosition, delta);
	}
}
