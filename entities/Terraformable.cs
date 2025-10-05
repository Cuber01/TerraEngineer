using Godot;
using TerraEngineer.entities.objects;

namespace TerraEngineer.entities;

public partial class Terraformable : Node2D
{
    [Export] private CollisionShape2D hitbox;
    [Export] public Biomes MyBiome;
    public bool Active = false;
    
    public virtual void Enable()
    {
        Active = true;
        hitbox.Disabled = false;
        Show();
    }
    
    public virtual void Disable()
    {
        Active = false;
        hitbox.Disabled = true;
        Hide();
    }
}