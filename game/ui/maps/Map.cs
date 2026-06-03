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
    private Vector2I baseOffset;
    private Vector2I calculateBaseOffset() => (MetSysApi.GetCurrentFlatCoords() - Size / 2);

    public override void _Ready()
    {
        // Cellular size is total size divided by cell size + shared borders.
        Size = (Vector2I)(GetSize() / MetSysApi.GetCellSizeOffset());
        mapView = MetSysApi.MakeMapView(this, -Size / 2, Size, 0);
        
        
        playerLocation = MetSysApi.AddPlayerLocation(this);
        controller = new Controller();
        controller.AddAction(Names.Actions.Quit, close);
        controller.AddAction(Names.Actions.OpenMap, close);
        controller.AddAction(Names.Actions.Left, () => moveOffset(Vector2I.Left));
        controller.AddAction(Names.Actions.Right, () => moveOffset(Vector2I.Right));
        controller.AddAction(Names.Actions.Up, () => moveOffset(Vector2I.Up));
        controller.AddAction(Names.Actions.Down, () => moveOffset(Vector2I.Down));
        
    }

    public override void _Process(double delta)
    {
        controller.Update();
    }

    private void moveOffset(Vector2I extraOffset)
    {
        mapView.Move(extraOffset);
        UpdateOffset(extraOffset);
    }

    private void UpdateOffset(Vector2I extraOffset)
    {
        offset += extraOffset;
        playerLocation.Set(Names.MetSys.Offset,
            -new Vector2(offset.X, offset.Y) * MetSysApi.GetCellSizeOffset());
        mapView.MoveTo(new Vector3I(offset.X, offset.Y, MetSysApi.CurrentLayer));
        
        Vector3I selectedCoords = MetSysApi.LastPlayerPosition + MathT.vec2ToVec3(offset - baseOffset);
        if (MetSysApi.IsCellDiscovered(selectedCoords))
        {
            textLabel.Text = MetSysApi.GetBiomeName(selectedCoords);    
        }
        else
        {
            textLabel.Text = Names.MetSys.BiomeNotFound;
        }
        
        
        mapView.UpdateAll();
    }

    private void open(Controller oldController)
    {
        oldController.SwitchControl(controller);
        Visible = true;
        GetTree().Paused = true; 
        baseOffset = calculateBaseOffset();
        offset = baseOffset;
        UpdateOffset(Vector2I.Zero);
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

