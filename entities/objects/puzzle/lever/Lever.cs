using Godot;
using TerraEngineer;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.objects;
using TerraEngineer.entities.objects.puzzle;

[Tool]
public partial class Lever : Entity, ISwitcher, IInteractable
{
    public event ISwitcher.SwitchedEventHandler Switched;
    public bool SwitchedOn { get; set; }
    public bool InteractionBlocked { get; set; }

    public override void _Ready()
    {
        MakeShaderUnique();
        InitSpriteWrapper();
    }

    public void OnInteracted()
    {
        SpriteWrapper.SetFrame(SwitchedOn ? 0 : 1);
        SwitchedOn = !SwitchedOn;
        Switched?.Invoke(SwitchedOn);
    }
}
