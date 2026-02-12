using Godot;
using System;
using System.Net.Mime;
using System.Xml;
using TerraEngineer;
using TerraEngineer.game;
using TerraEngineer.ui.player_hud;
using TerraEngineer.ui.textbox;

public partial class Popup : Node2D, IPopupable
{
    public Controller Controller { get; set; }
    
    [Export] private RichTextLabel label;
    [Export] private Sprite2D popupBoxSprite;
    
    private readonly Vector2 screenSize = new Vector2(320, 190);
    private bool closingAllowed = false;
    
    public override void _Ready()
    {
        // Set position so that the box is centered
        Vector2 size = popupBoxSprite.Texture.GetSize();
        GlobalPosition = new Vector2( MathF.Floor((320-size.X)/2)  , MathF.Floor((190-size.Y)/2) );
        
        SetupControls();
    }
    
    public override void _Process(double delta)
    {
        Controller.Update((float)delta);
    }
    
    public void ShowPopup(StringName message)
    {
        label.Text = message;
       
        Visible = true;
        GetTree().Paused = true;
        closingAllowed = false;
    }
    
    public void SetupControls()
    {
        Controller = new Controller();
        Controller.AddAction(Names.Actions.Attack, Close);
    }

    public void Close()
    {
        Controller.GiveBackControl();
        Visible = false;
        GetTree().Paused = false;
    }


}
