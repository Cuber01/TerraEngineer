using Godot;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.objects;

namespace TerraEngineer.entities;

public partial class TerraformableEntity : Entity, ITerraformable
{
    [Export] public CollisionShape2D Hitbox;
    [Export] public Biomes MyBiome { get; set; }
    public bool Active { get; set; }
    public TerraformableCaretaker Caretaker { get; set; }

    public virtual void Update(float delta)
    {
        CM?.UpdateComponents(delta);
        HandleMove();
    }
    
    public override void _PhysicsProcess(double delta)
    {
        if (Active)
        {
            Update((float)delta);   
        }
    }
    
    public virtual void Enable()
    {
        Active = true;
        if (Hitbox != null)
        {
            Hitbox.Disabled = false;
        }
        Show();
    }
    
    public virtual void Disable()
    {
        Active = false;
        if (Hitbox != null)
        {
            Hitbox.Disabled = true;
        }
        Hide();
    }

    public override void HandleMove()
    {
        if (velocity != Vector2.Zero)
        {
            Caretaker.GlobalPosition = GlobalPosition;
            Position = Vector2.Zero;
        }
            
        base.HandleMove();
    }
}