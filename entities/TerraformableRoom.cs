using Godot;
using TerraEngineer.entities.objects;

namespace TerraEngineer.entities;

public partial class TerraformableRoom : Node2D, ITerraformable
{
    [Export] public Biomes MyBiome { get; set; }
    [Export] public bool Active { get; set; }
    public TerraformableCaretaker Caretaker { get; set; }
    
    public void Enable()
    {
        foreach (Node child in GetChildren())
        {
            child.ProcessMode = ProcessModeEnum.Disabled;
        }
        Show();
    }
    
    public void Disable()
    {
        foreach (Node child in GetChildren())
        {
            child.ProcessMode = ProcessModeEnum.Inherit;
        }
        Hide();
    }
}