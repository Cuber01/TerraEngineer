using Godot;
using TENamespace.health;
using TerraEngineer.entities.mobs;

namespace TerraEngineer.entities.tiles.terra_spike;

public partial class Spike : TerraformableEntity
{
    [Export] private Area2D hurtArea;

    private const int Damage = 2;

    public override void Init()
    {
        Direction4 facing = ((TerraformableSpikeCaretaker)Caretaker).SpikeFacing;
        SpriteWrapper.Init(Sprite);
        SpriteWrapper.SetFrame(directionToFrame(facing));

        float rotation = getRotationForDirection(facing);
        
        if (hurtArea != null)
        {
            hurtArea.Rotation = rotation;
            hurtArea.BodyEntered += onBodyEntered;
        }

        if (Hitbox != null)
        {
            Hitbox.Rotation = rotation;
        }
    }
    
    public override void Enable()
    {
        base.Enable();
        if (hurtArea != null)
        {
            hurtArea.Monitoring = true;
        }
    }
    
    public override void Disable()
    {
        base.Disable();
        if (hurtArea != null)
        {
            hurtArea.Monitoring = false;
        }
    }

    private void onBodyEntered(Node2D stepper)
    {
        if (stepper is Player p)
        {
            p.CM.GetComponent<Health>().ChangeHealth(-Damage, this);
            p.ReturnToHazardRespawnPoint();
        }
        else if (stepper is Entity e)
            e.Die();
    }

    // Direction4 is missing 0
    private static int directionToFrame(Direction4 facing) 
    { 
        return  (int)facing == -1 ? 0 : (int)facing;
    }

    private static float getRotationForDirection(Direction4 facing) => facing switch
    {
        Direction4.Down  => Mathf.Pi,     // 180 deg
        Direction4.Up    => 0f,
        Direction4.Right => -Mathf.Pi / 2, // -90 deg
        Direction4.Left  => Mathf.Pi / 2,  // 90 deg
        _ => 0f
    };
    
}