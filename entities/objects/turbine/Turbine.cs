using Godot;
using System;
using TerraEngineer.entities.mobs;

namespace TerraEngineer.entities.objects;

public partial class Turbine : Entity
{
	[Export] private Area2D areaOfEffect;
	[Export] private PackedScene particleScene;
	private const float UpwardForce = 100f;
	private GpuParticles2D particles;
	
	public override void _Ready()
	{
		base._Ready();
		
		#if TOOLS
		if(Engine.IsEditorHint())
			return;
		#endif
	}
	
	private void onAreaBodyEntered(Node2D body)
	{
		if (body is Entity entity)
		{
			entity.velocity = entity.velocity with { Y = -UpwardForce };
		}
	}
}

