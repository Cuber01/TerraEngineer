using Godot;
using System;
using TENamespace.health;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.mobs.creatures;

[Tool]
public partial class HPGateway : Entity
{
	enum Mode
	{
		Heal=0,
		Hurt=1
	}

	[Export] private Mode ExportedMode
	{
		get => _mode;
		set
		{
			_mode = value;
			updateSprite();
		}
	}
	private Mode _mode = Mode.Heal;
	
	public override void _Ready()
	{
		updateSprite();
	}

	private void updateSprite()
		=> Sprite.Frame = (int)_mode;

	private void onCreatureEntered(Node2D creature)
	{
		Health healthComp = ((Creature)creature).CM.TryGetComponent<Health>();
		if (_mode == Mode.Heal)
		{
			healthComp.FullHeal();	
		}
		else
		{
			if (healthComp.HP == 1)
			{
				healthComp.ChangeHealth(-1); // Kill
			}
			else
			{
				healthComp.ChangeHealth(1 - healthComp.HP); // Set HP to 1
			}
		}
		
	}
}
