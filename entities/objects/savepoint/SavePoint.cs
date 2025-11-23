using Godot;

namespace TerraEngineer.entities.objects.savepoint;

public partial class SavePoint : Node2D
{
    private void onPlayerEntered(Node2D body)
    {
        SaveData.SetValue(Names.SaveSections.SavePointData,
                                        Names.SaveSections.SavePointPosition,
                                        GlobalPosition);
        
        StringName levelName = (StringName)GetParent().GetMeta(Names.Properties.LevelName);
        
        SaveData.SetValue(Names.SaveSections.SavePointData,
                            Names.SaveSections.SavePointLevel,
                            levelName);
        
        SaveData.WriteChanges();
    }
}