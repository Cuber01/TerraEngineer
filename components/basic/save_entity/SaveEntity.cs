using Godot;
using TerraEngineer.entities.mobs;

namespace TENamespace.save_entity;

public partial class SaveEntity : Component
{
    [Export] private Mob actor;
    [Export] private string saveSection;
    [Export] private string saveName;

    public override void _Ready()
    {
        bool exists = (bool)SaveData.ReadValue(saveSection, saveName);
        if (!exists) 
        {
            CallDeferred(Node.MethodName.QueueFree);
        }
    }

    public void ChangeState(bool exists)
    {
        SaveData.SetValue(saveSection, saveName, exists);
    }
}