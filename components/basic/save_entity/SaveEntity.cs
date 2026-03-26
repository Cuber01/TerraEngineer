using System;
using Godot;
using TerraEngineer;
using TerraEngineer.entities.mobs;

namespace TENamespace.save_entity;

public partial class SaveEntity : Component
{
    [Export] private StringName saveName;
    private StringName saveSection;

    public Action<Node2D> DoIfTrue = (Node2D actor) =>
    {
        actor.CallDeferred(Node.MethodName.QueueFree);
    };

    public override void OptionalInit(Node2D actor)
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

    public void Setup(StringName saveName, Action<Node2D> doIfTrue)
    {
        this.DoIfTrue = doIfTrue;
        this.saveName = saveName;
    }

    public void ChangeState(bool removed)
    {
        SaveData.SetAddValue(saveSection, saveName, removed);
    }
}