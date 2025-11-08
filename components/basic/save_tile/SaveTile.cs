using Godot;

namespace TENamespace.basic.save_tile;

public partial class SaveTile : Component
{
    private string levelName;
    private const string TilesKey = "removed_tiles";

    public override void _Ready()
    {
        levelName = (string)GetParent().GetMeta("level_name");
    }
    
    public void ChangeState(Vector2I myCoords, bool exists)
    {
        if (exists)
        {
            SaveData.RemoveValueInArray(levelName, TilesKey,  myCoords); 
        }
        else
        {
            SaveData.AddValueToArray(levelName, TilesKey,  myCoords);       
        }
    }
}