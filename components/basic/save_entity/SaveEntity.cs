using System;
using Godot;
using TerraEngineer;
using TerraEngineer.entities.mobs;

namespace TENamespace.save_entity;

public partial class SaveEntity : Component
{
    [Export] private StringName saveName;
    private StringName saveSection;

    public Action<Entity> doIfFalse = (Entity actor) =>
    {
        actor.CallDeferred(Node.MethodName.QueueFree);
    };

    public override void OptionalInit(Entity actor)
    {
        saveSection = (StringName)Actor.GetParent().GetMeta(Names.Properties.LevelName);
        if (saveSection == "" || saveSection == null)
        {
            throw new Exception("No level name found.");
        }

        if (saveName =="" || saveName is null)
        {
            throw new Exception("Save name is empty.");
        }
        
        bool exists = (bool)SaveData.ReadValue(saveSection, saveName);
        if (!exists) 
        {
           doIfFalse.Invoke(actor);
        }
    }

    public void Setup(StringName saveName, Action<Entity> doIfFalse)
    {
        this.doIfFalse = doIfFalse;
        this.saveName = saveName;
    }

    public void ChangeState(bool exists)
    {
        SaveData.SetValue(saveSection, saveName, exists);
    }
}