using Godot;
using TerraEngineer.entities.mobs;

namespace TENamespace.basic.save_tile;

public partial class SaveTile : Component
{
    private string levelName;
    private const string TilesKey = "removed_tiles";
    

    public override void Init(Entity actor)
    {
        base.Init(actor);
        levelName = (string)Actor.GetParent().GetMeta("level_name");
    }

    public void ChangeState(Vector2I myCoords, bool exists)
    {
        if (exists)
        {
            SaveData.RemoveValueInArray(levelName, TilesKey,  myCoords.ToString()); 
        }
        else
        {
            SaveData.AddValueToArray(levelName, TilesKey,  myCoords.ToString());       
        }
    }
}