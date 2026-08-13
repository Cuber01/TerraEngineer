using Godot;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.objects;

namespace TerraEngineer.entities.tiles;

public partial class TerraformableTile : Entity, ITerraformable
{
    [Export] protected CollisionShape2D CollisionShape;
    [Export] public Biomes MyBiome { get; set; }
    
    public bool Active { get; set; }
    public TerraformableCaretaker Caretaker { get; set; }
    
    public virtual void Enable()
    {
        Show();
        CollisionShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
    }
    
    public virtual void Disable()
    {
        Hide();
        CollisionShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
    }
}