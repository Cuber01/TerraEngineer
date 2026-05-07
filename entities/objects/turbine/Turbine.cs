using Godot;
using System;
using System.Collections.Generic;
using TerraEngineer.entities.mobs;

namespace TerraEngineer.entities.objects;

public partial class Turbine : Entity
{
	[Export] private Area2D areaOfEffect;
	private List<Entity> affectedEntities = new List<Entity>();
	private const float UpwardForce = 1f;
	private const float maxVelocity = -100f;
	
	private GpuParticles2D particles;
	
	public override void _Ready()
	{
		base._Ready();
		
		#if TOOLS
		if(Engine.IsEditorHint())
			return;
		#endif
	}

	public override void _PhysicsProcess(double delta)
	{
		foreach (Entity e in affectedEntities)
		{
			e.velocity = new Vector2(e.velocity.X, Math.Min(e.velocity.Y - UpwardForce, maxVelocity));
		}
 	}
	
	private void onAreaBodyEntered(Node2D body)
	{
		if (body is Entity entity)
		{
			affectedEntities.Add(entity);
		}
	}

	private void onAreaBodyExited(Node2D body)
	{
		if (affectedEntities.Contains(body as Entity))
			affectedEntities.Remove(body as Entity);
	}
	
}

