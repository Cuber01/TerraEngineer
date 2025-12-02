using Godot;

namespace TerraEngineer.entities.objects.puzzle;

[Tool]
public partial class Button : Node2D, ISwitcher
{
    public event ISwitcher.SwitchedEventHandler Switched;
    public bool SwitchedOn { get; set; }

    public override void _Ready()
    {
        Material mat = (Material)GetNode<AnimatedSprite2D>(Names.Node.AnimatedSprite2D).Material.Duplicate(true);
        GetNode<AnimatedSprite2D>(Names.Node.AnimatedSprite2D).Material = mat;
    }
    
    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("f2"))
        {
            SwitchedOn = !SwitchedOn;
            Switched?.Invoke(SwitchedOn);
        }
    }
}