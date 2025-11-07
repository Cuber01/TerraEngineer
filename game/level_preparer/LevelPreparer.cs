using Godot;
using System;
using Godot.Collections;

public partial class LevelPreparer : Node2D
{
    public override void _Ready()
    {
        
    }

    public void Prepare()
    {
        TileMapLayer specialLayer = GetNode<TileMapLayer>("Level/SpecialTiles");
        Array<Vector2I> specialTiles = specialLayer.GetUsedCells();

        foreach (Vector2I coords in specialTiles)
        {
            TileData data = specialLayer.GetCellTileData(coords);
            spawnTile( (string)data.GetCustomData("special_type"), coords);
        }
    }

    private void spawnTile(string name, Vector2I coords)
    {
        
    }
    

}
