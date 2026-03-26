using Godot;
using TerraEngineer;
using TerraEngineer.entities.mobs;

namespace TENamespace.basic.save_tile;

public partial class SaveTile : Component
{
    private StringName levelName;

    public override void Init(Node2D actor)
    {
        base.Init(actor);
        levelName = (StringName)Actor.GetParent().GetMeta(Names.Properties.LevelName);
    }

    public void ChangeState(Vector2I myCoords, bool removed)
    {
        Variant asArray = SaveData.VecToParseableArray(myCoords);
        
        if (!removed)
        {
            SaveData.RemoveValueInArray(levelName, Names.SaveSections.RemovedTiles,  asArray); 
        }
        else
        {
            SaveData.AddValueToArray(levelName, Names.SaveSections.RemovedTiles,  asArray);       
        }
    }
}