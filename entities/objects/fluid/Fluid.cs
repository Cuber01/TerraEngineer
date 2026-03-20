using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public partial class Fluid : Node2D
{
	[Export] private Vector2I size = new Vector2I(32, 32);
	[Export] private int springsAmountPer10Px = 1;
	[Export] private PackedScene fluidSpringScene;
	
	private List<FluidSpring> fluidSprings = new List<FluidSpring>();
	private bool initialized = false;
	
	public override void _Ready()
	{
		if (!initialized)
		{
			setup();
			initialized = true;	
		}
	}

	private void setup()
	{
		int springsAmount = (int)Math.Ceiling( (size.X / 10.0) * springsAmountPer10Px );
		int spaceBetween = size.X / springsAmount;
		
		int xOffset = 0;
		for (int i = 0; i < springsAmount; i++)
		{
			FluidSpring springInstance = (FluidSpring)fluidSpringScene.Instantiate();
			springInstance.Position = new Vector2(xOffset, 0);
			AddChild(springInstance);
			fluidSprings.Add(springInstance);
			
			xOffset += spaceBetween;
		}
	}

	private void reset()
	{
		foreach (FluidSpring spring in fluidSprings)
		{
			spring.QueueFree();
		}
		fluidSprings.Clear();
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
