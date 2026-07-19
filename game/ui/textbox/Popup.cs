using Godot;
using System;
using TerraEngineer;
using TerraEngineer.game;
using TerraEngineer.game.ui;
using TerraEngineer.ui.textbox;

public partial class Popup : Node2D, IPopupable
{
    public InputContext InputContext { get; set; }
    
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
    
    public void ShowPopup(string message)
    {
        label.Text = message;
       
        Visible = true;
        GetTree().Paused = true;
        closingAllowed = false;
    }
    
    public void SetupControls()
    {
        InputContext = new InputContext();
        InputContext.AddAction(Names.Actions.Attack, Close);
    }

    public void Close()
    {
        InputStackManager.Pop();
        Visible = false;
        GetTree().Paused = false;
    }


}
