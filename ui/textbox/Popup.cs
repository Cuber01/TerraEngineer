using Godot;
using System;
using System.Xml;
using TerraEngineer;
using TerraEngineer.ui.textbox;

public partial class Popup : Node2D, IPopupable
{
    [Export] private Sprite2D popupBoxSprite;
    private readonly Vector2 screenSize = new Vector2(320, 190);
    private bool closingAllowed = false;
    
    public override void _Ready()
    {
        // Set position so that the box is centered
        Vector2 size = popupBoxSprite.Texture.GetSize();
        GlobalPosition = new Vector2( MathF.Floor((320-size.X)/2)  , MathF.Floor((190-size.Y)/2) );
    }
    
    public override void _Process(double delta)
    {
        if (Input.IsActionJustReleased(Names.Actions.Attack))
        {
            // To stop the window from auto-closing on open
            closingAllowed = true;
        }
            
        if (Input.IsActionJustPressed(Names.Actions.Attack) && closingAllowed)
        {
            Visible = false;
            GetTree().Paused = false;
        }
    }
    
    public void Display()
    {
        Visible = true;
        GetTree().Paused = true;
        closingAllowed = false;
    }
    
    public void Close()
    {
        Visible = false;
        GetTree().Paused = false;
    }
}
