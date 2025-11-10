using System;
using Godot;
using TerraEngineer;
using TerraEngineer.entities.mobs;

namespace TENamespace.save_entity;

public partial class SaveEntity : Component
{
    [Export] private StringName saveName;
    private StringName saveSection;

    public override void Init(Entity actor)
    {
        base.Init(actor);
        
        saveSection = (StringName)Actor.GetParent().GetMeta(Names.Properties.LevelName);
        if (saveSection == "")
        {
            throw new Exception("No level name found.");
        }
        
        bool exists = (bool)SaveData.ReadValue(saveSection, saveName);
        if (!exists) 
        {
            actor.CallDeferred(Node.MethodName.QueueFree);
        }
    }

    public void Setup(StringName saveSection, StringName saveName)
    {
        this.saveSection = saveSection;
        this.saveName = saveName;
    }

    public void ChangeState(bool exists)
    {
        SaveData.SetValue(saveSection, saveName, exists);
    }
}