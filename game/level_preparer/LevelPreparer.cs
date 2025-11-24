using Godot;
using System.Linq;
using Godot.Collections;
using TerraEngineer;
using TerraEngineer.entities.tiles;
using Array = Godot.Collections.Array;

public partial class LevelPreparer : Node2D
{
    // These are merged into a dictionary at runtime. Make sure that the corresponding name and PackedScene have the same index!
    [Export] private StringName[] tileNames;
    [Export] private PackedScene[] tileScenes;
    
    private Dictionary<StringName, PackedScene> tilesDict;
    
    public override void _Ready()
    {
        tilesDict = new Dictionary<StringName, PackedScene>();
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
        
        StringName levelName = (StringName)newLevel.GetMeta(Names.Properties.LevelName);
        if(levelName == "")
            return;
        
        Array<Vector2I> specialTiles = specialLayer.GetUsedCells();

        Array removedTiles = (Array)SaveData.ReadValue(levelName, Names.SaveSections.RemovedTiles);
        foreach (Vector2I coords in specialTiles)
        {
            Variant asArray = SaveData.VecToParseableArray(coords);
            if (!removedTiles.Contains(asArray))
            {
                TileData data = specialLayer.GetCellTileData(coords);
                spawnTile( (StringName)data.GetCustomData(Names.Properties.SpecialType), 
                                coords,
                                newLevel, specialLayer);
            }
        }

        specialLayer.CallDeferred(Node.MethodName.QueueFree);
    }


    private void spawnTile(StringName name, Vector2I mapCoords, Node2D level, TileMapLayer dataLayer)
    {
        PackedScene scene = tilesDict[name];
        Tile instance = (Tile)scene.Instantiate();
        instance.MapCoords = mapCoords;
        instance.GlobalPosition = dataLayer.MapToLocal(mapCoords) + dataLayer.GlobalPosition;
        level.AddChild(instance);
    }
    
}
