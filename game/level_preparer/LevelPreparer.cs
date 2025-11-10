using Godot;
using System.Linq;
using Godot.Collections;
using TerraEngineer;
using TerraEngineer.entities.tiles;
using Array = Godot.Collections.Array;

public partial class LevelPreparer : Node2D
{
    // These are merged into a dictionary at runtime. Make sure that the corresponding name and PackedScene have the same index!
    [Export] private string[] tileNames;
    [Export] private PackedScene[] tileScenes;
    
    private Dictionary<string, PackedScene> tilesDict;
    
    public override void _Ready()
    {
        tilesDict = new Dictionary<string, PackedScene>();
        int i = 0;
        while (i < tileNames.Length)
        {
            tilesDict.Add(tileNames[i], tileScenes[i]);
            i++;
        }
    }

    public void Prepare(Node2D newLevel)
    {
        TileMapLayer specialLayer = newLevel.GetNodeOrNull<TileMapLayer>(Names.Node.SpecialTiles);
        if(specialLayer == null)
            return;
        
        string levelName = (string)newLevel.GetMeta(Names.Properties.LevelName);
        if(levelName == "")
            return;
        
        Array<Vector2I> specialTiles = specialLayer.GetUsedCells();

        Array removedTiles = (Array)SaveData.ReadValue(levelName, Names.SaveSections.RemovedTiles);
        foreach (Vector2I coords in specialTiles)
        {
            Variant asArray = VecToParseableArray(coords);
            if (!removedTiles.Contains(asArray))
            {
                TileData data = specialLayer.GetCellTileData(coords);
                spawnTile( (string)data.GetCustomData(Names.Properties.SpecialType), 
                                coords,
                                newLevel, specialLayer);
            }
        }

        specialLayer.CallDeferred(Node.MethodName.QueueFree);
    }

    // We need to convert it to a really weird array in order for the comparison to work...
    public static Array VecToParseableArray(Vector2I coords)
        => new Array() {(float)coords.X, (float)coords.Y};

    private void spawnTile(string name, Vector2I mapCoords, Node2D level, TileMapLayer dataLayer)
    {
        PackedScene scene = tilesDict[name];
        Tile instance = (Tile)scene.Instantiate();
        instance.MapCoords = mapCoords;
        instance.GlobalPosition = dataLayer.MapToLocal(mapCoords) + dataLayer.GlobalPosition;
        level.AddChild(instance);
    }
    
}
