using Godot;
using System.Runtime.CompilerServices;
using TerraEngineer.game;
using TerraEngineer.game.ui;
using TerraEngineer.ui.player_hud;

namespace TerraEngineer.ui.maps;

public partial class Map : Control, IUserInterface
{
    public event IUserInterface.ClosedInternallyEventHandler ClosedInternally;
 
    [Export] private RichTextLabel textLabel;
    private InputContext openMapContext;
    private InputContext teleporterMapContext;

    private const int TeleporterIconId = 7;
    private bool isTeleporterMap = false;
    public bool IsOpen { get; set; }
    
    // The size of the window in cells.
    private new Vector2I Size { get; set; }

    // MapView object that draws cells.
    private GodotObject mapView;

    private Node2D worldManager;
    private Node2D playerLocation;
    private Vector2I offset;
    private Vector2I baseOffset;
    private Vector2I calculateBaseOffset() => (MetSysApi.GetCurrentFlatCoords() - Size / 2);
    
    private static readonly Vector2I mapCellSize = new Vector2I(40, 16);
    private Vector3I selectedCoords;

    private Player player;

    public override void _Ready()
    {
        player = GetNode<Player>(Names.NodePaths.Player);
        worldManager = GetNode<Node2D>(Names.NodePaths.WorldManager);
        
        // Cellular size is total size divided by cell size + shared borders.
        Size = (Vector2I)(GetSize() / MetSysApi.GetCellSizeOffset());
        mapView = MetSysApi.MakeMapView(this, Vector2I.Zero, mapCellSize, 0);
        
        playerLocation = MetSysApi.AddPlayerLocation(this);
        
        openMapContext = new InputContext();
        openMapContext.AddAction(Names.Actions.Left, () => moveOffset(Vector2I.Left));
        openMapContext.AddAction(Names.Actions.Right, () => moveOffset(Vector2I.Right));
        openMapContext.AddAction(Names.Actions.Up, () => moveOffset(Vector2I.Up));
        openMapContext.AddAction(Names.Actions.Down, () => moveOffset(Vector2I.Down));

        teleporterMapContext = new InputContext();
        openMapContext.AddAction(Names.Actions.Attack, chooseTeleporter);
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
        
        selectedCoords = MetSysApi.LastPlayerPosition + MathT.vec2ToVec3(offset - baseOffset);
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

    private void chooseTeleporter()
    {
        if (selectedCoords == MetSysApi.CurrentCoords)
        {
            ClosedInternally?.Invoke(this);
            return;
        }

        if (MetSysApi.GetMarkerAt(selectedCoords) == TeleporterIconId)
        {
            worldManager.Call("teleport", selectedCoords);
            ClosedInternally?.Invoke(this);
        }
        else
        {
            // Do some buzz sound.
            GD.Print("No teleporter there.");
        }
    }

    public void Open()
    {
        handleOpen();
    }

    public void OpenForTeleporter()
    {
        isTeleporterMap = true;
        InputStackManager.Push(teleporterMapContext);
        handleOpen();
    }

    private void handleOpen()
    {
        InputStackManager.Push(openMapContext);
        GetParent<Node2D>().Show();
        GetTree().Paused = true; 
        baseOffset = calculateBaseOffset();
        offset = baseOffset;
        UpdateOffset(Vector2I.Zero);
        IsOpen = true;   
    }
    
    public void Close()
    {
        if (isTeleporterMap)
        {
            InputStackManager.Pop();
            isTeleporterMap = false;
        }
        
        InputStackManager.Pop();
        GetParent<Node2D>().Hide();
        GetTree().Paused = false; 
        IsOpen = false;
    }
}

