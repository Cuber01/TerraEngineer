using Godot;
using TENamespace;
using TerraEngineer.entities;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.objects;

public partial class Trampoline : TerraformableEntity
{
    [Export] private float bounceVelocity = 150;
    
    private void onBodyEntered(Node2D body)
    {
        Entity e = (Entity)body;
        if (e.velocity.Y > 0)
        {
            e.velocity.Y = -bounceVelocity;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        CM?.UpdateComponents((float)delta);
        HandleMove();
    }
    
}
