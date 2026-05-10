using Godot;
using Godot.Collections;

namespace TerraEngineer.entities.projectiles;

public partial class RaycastBullet : Projectile
{
    public override void _PhysicsProcess(double delta)
    {
        CM.UpdateComponents((float)delta);
        
        CM.GetComponent<FreeFly>().FlyInDirection(DirectionNormal, (float)delta);
        
        collisionDetection((float)delta);
        
        HandleMove();
    }

    // TODO rewrite this using raycast2d
    private void collisionDetection(float delta)
    {
        PhysicsDirectSpaceState2D spaceState = GetWorld2D().DirectSpaceState;
        
        var query = PhysicsRayQueryParameters2D.Create(GlobalPosition, GlobalPosition + velocity * delta);
        query.CollisionMask = (1 << 0) | (1 << 2) | (1 << 3); // Ground, enemy, object
        
        Dictionary result = spaceState.IntersectRay(query);
        if (result.Count > 0)
        {
            Vector2 hitPos = (Vector2)result["position"];
            Node2D collider = (Node2D)result["collider"];

            GlobalPosition = hitPos;
            velocity = Vector2.Zero;
            OnHit(collider);
        }
    }
}