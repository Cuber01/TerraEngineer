using System;
using Godot;
using TerraEngineer;
using TerraEngineer.entities.mobs;

namespace TENamespace.save_entity;

public partial class SaveEntity : Component
{
    [Export] private StringName saveName;
    private StringName saveSection;

    public Action<Entity> DoIfTrue = (Entity actor) =>
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
        
        Variant exists = SaveData.ReadValue(saveSection, saveName);
        if (exists.VariantType == Variant.Type.Bool && (bool)exists == true) 
        {
           DoIfTrue.Invoke(actor);
        }
    }

    public void Setup(StringName saveName, Action<Entity> doIfTrue)
    {
        this.DoIfTrue = doIfTrue;
        this.saveName = saveName;
    }

    public void ChangeState(bool exists)
    {
        SaveData.SetAddValue(saveSection, saveName, exists);
    }
}