using Godot;
using System;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.objects;

namespace TerraEngineer.entities;

public partial class Terraformable : Entity
{
    [Export] public CollisionShape2D Hitbox;
    [Export] public Biomes MyBiome;
    public bool Active;
    public TerraformableCaretaker Caretaker;
    
    public virtual void Setup(TerraformableCaretaker caretaker)
    {
        Caretaker = caretaker;
    }

    public virtual void Update(float delta)
    {
        CM?.UpdateComponents(delta);
        HandleMove();
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
        if(velocity != Vector2.Zero)
            Caretaker.GlobalPosition = GlobalPosition;
        base.HandleMove();
    }
}