using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using TENamespace;

public partial class FindClosestNode : Component
{
	[ExportGroup("External")]
	
	[Export] public bool LookForBodies = true;
	// Warning: This component assumes parent of Area2D is the target
	[Export] public bool LookForAreas = true;

	[ExportGroup("Internal")] 
	
	[Export] private Area2D myArea;

	private List<Node2D> nodes = new List<Node2D>();
	
	#region setup
	public override void _Ready()
	{
		myArea.AreaEntered += onAreaEntered;
		myArea.AreaExited += onAreaExited;
		myArea.BodyEntered += onBodyEntered;
		myArea.BodyExited += onBodyExited;
	}

	private void onAreaEntered(Area2D area)
	{
		nodes.Add(area.GetParent<Node2D>());
	}

	private void onAreaExited(Area2D area)
	{
		nodes.Remove(area.GetParent<Node2D>());
	}
	
	private void onBodyEntered(Node2D body)
	{
		nodes.Add(body);
	}

	private void onBodyExited(Node2D body)
	{
		nodes.Remove(body);
	}
	#endregion

	public Vector2? FindClosest()
	{
		Node2D closest = nodes.OrderBy(p => p.GlobalPosition.DistanceSquaredTo(Actor.GlobalPosition)).FirstOrDefault();
		if (closest == null)
		{
			return null;
		}
		else
		{
			return closest.GlobalPosition;
		}
	}
}
