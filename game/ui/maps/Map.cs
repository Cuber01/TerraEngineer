using Godot;
using System.Runtime.CompilerServices;
using TerraEngineer.game;
using TerraEngineer.ui.player_hud;

namespace TerraEngineer.ui.maps;

public partial class Map : Control, IConnectable<Player>
{
    [Export] private RichTextLabel textLabel;
    private Controller controller;
    
    // The size of the window in cells.
    private new Vector2I Size { get; set; }

    // MapView object that draws cells.
    private GodotObject mapView;

    private Node2D playerLocation;
    private Vector2I offset;

    public override void _Ready()
    {
        // Cellular size is total size divided by cell size + shared borders.
        Size = (Vector2I)(GetSize() / MetSysApi.GetCellSizeOffset());
        mapView = MetSysApi.MakeMapView(this, -Size / 2, Size, 0);
        
        playerLocation = MetSysApi.AddPlayerLocation(this);
        controller = new Controller();
        controller.AddAction(Names.Actions.Quit, close);
        controller.AddAction(Names.Actions.OpenMap, close);
    }

    public override void _Process(double delta)
    {
        controller.Update();
    }

    // TODO this needs to be implemented alongside our controller input system instead of this
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent)
        {
            if (keyEvent.Pressed)
            {
                // Move with arrow keys.
                Vector2I moveOffset = Vector2I.Zero;
                    
                if (keyEvent.Keycode == Key.Left)
                {
                    moveOffset = Vector2I.Left;
                }
                else if (keyEvent.Keycode == Key.Right)
                {
                    moveOffset = Vector2I.Right;
                }
                else if (keyEvent.Keycode == Key.Up)
                {
                    moveOffset = Vector2I.Up;
                }
                else if (keyEvent.Keycode == Key.Down)
                {
                    moveOffset = Vector2I.Down;
                }
                    
                if (moveOffset != Vector2I.Zero)
                {
                    mapView.Move(moveOffset);
                    UpdateOffset(new Vector3I(moveOffset.X, moveOffset.Y, 0));
                }
            }
        }
    }

    private void UpdateOffset(Vector3I _extraOffset)
    {
        offset = MetSysApi.GetCurrentFlatCoords() - Size / 2;
        playerLocation.Set(Names.MetSys.Offset,
            -new Vector2(offset.X, offset.Y) * MetSysApi.GetCellSizeOffset());
        mapView.MoveTo(new Vector3I(offset.X, offset.Y, MetSysApi.CurrentLayer));
        textLabel.Text = MetSysApi.GetBiomeName(MetSysApi.LastPlayerPosition);
        mapView.UpdateAll();
    }

    private void open(Controller oldController)
    {
        oldController.SwitchControl(controller);
        Visible = true;
        GetTree().Paused = true; 
        UpdateOffset(Vector3I.Zero);
    }
    
    private void close()
    {
        controller.GiveBackControl();
        Visible = false;
        GetTree().Paused = false; 
    }
    
    public void Connect(Player actor)
    {
        actor.OpenMap += open;
    }
    
    public void Disconnect(Player actor)
    {
        throw new System.NotImplementedException();
    }
}

