using Godot;
using TerraEngineer.entities.mobs;

namespace TerraEngineer.entities.objects.puzzle;

[Tool]
public partial class Button : Entity, ISwitcher
{
    public event ISwitcher.SwitchedEventHandler Switched;
    public bool SwitchedOn { get; set; }

    private int bodiesOnButton = 0;
    
    public override void _Ready()
    {
        MakeShaderUnique();
    }

    private void onBodyEntered(Node2D _)
    {
        bodiesOnButton++;
        if (!SwitchedOn)
        {
            Sprite.Frame = 1;
            SwitchedOn = true;
            Switched?.Invoke(true);
        }
    }

    private void onBodyExited(Node2D _)
    {
        bodiesOnButton--;

        if (bodiesOnButton == 0)
        {
            Sprite.Frame = 0;
            SwitchedOn = false;
            Switched?.Invoke(false);
        }
        
    }
    
}