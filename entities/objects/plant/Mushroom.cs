using Godot;
using TerraEngineer.entities;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.objects;

public partial class Mushroom : Terraformable
{
    private const float BounceVelocity = 150;
    
    private void onBodyEntered(Node2D body)
    {
        Entity e = (Entity)body;
        if (e.velocity.Y > 0)
        {
            e.velocity.Y = -BounceVelocity;
        }
    }
    
}
