using Godot;
using System;
using TENamespace.basic.builders.gravity_bullet_spawner;
using TerraEngineer;
using TerraEngineer.entities.mobs.creatures;

public partial class Ufo : Creature
{
    private readonly Vector2 distanceToKeepFromPlayer = new Vector2(0, -10);
    private Player player;
    
    public override void Init()
    {
        player = GetNode<Player>(Names.NodePaths.Player);
        TimerManager.Schedule(2.5f, true, this, _ =>
        {
            CM.GetComponent<GravityBulletSpawner>()
                .Start()
                .SetPosition(GlobalPosition)
                .Build();

            CM.GetComponent<GravityBulletSpawner>().AddToGame();
        });
    }
    
    public override void _PhysicsProcess(double delta)
    {
        CM.GetComponent<FreeFly>().FlyToPoint(player.GlobalPosition + distanceToKeepFromPlayer, (float)delta);
        CM.UpdateComponents((float)delta);
        
        HandleMove();
        FlipIfHitWall();
    }

}
