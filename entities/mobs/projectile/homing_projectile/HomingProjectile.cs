using Godot;
using System;
using TerraEngineer;
using TerraEngineer.entities.projectiles;

public partial class HomingProjectile : Projectile
{
	private Player player;
	
	public override void _Ready()
	{
		player = GetNode<Player>(Names.NodePaths.Player);
	}

	public override void _PhysicsProcess(double delta)
	{
		CM.UpdateComponents((float)delta);
        
		CM.GetComponent<FreeFly>().FlyToPoint(player.GlobalPosition, (float)delta);
        
		HandleMove();
	}
}
