using Godot;
using TerraEngineer.entities;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.objects;

public partial class Mushroom : Terraformable
{
    private const float BounceVelocity = 150;
    
    private void onBodyEntered(Node2D body)
    {
        Player player = (Player)body;
        if (player.velocity.Y > 0)
        {
            player.velocity.Y = -BounceVelocity;
        }
    }
    
}
