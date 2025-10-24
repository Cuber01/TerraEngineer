using Godot;
using System;

[Tool]
public partial class XBounds : Node2D
{
	[Export] private Node2D right;
	[Export] private Node2D left;

	public float XLeft;
	public float XRight;
	
	public override void _Ready()
	{
		XLeft = left.GlobalPosition.X;
		XRight = right.GlobalPosition.X;
	}
	
	public override void _Process(double delta)
	{
		if (Engine.IsEditorHint() && right != null && left != null)
		{
			QueueRedraw(); 
		}
	}
	
	public override void _Draw()
	{
		DrawLine(right.GlobalPosition, left.GlobalPosition, Colors.White);
		DrawCircle(new Vector2( (right.GlobalPosition.X + left.GlobalPosition.X)/2f ,right.GlobalPosition.Y),
			1, Colors.Aqua);
		base._Draw();
	}
}
