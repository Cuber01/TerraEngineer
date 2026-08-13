using Godot;
using TENamespace.basic.save_tile;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.objects;

namespace TerraEngineer.entities.tiles;

public partial class BreakableTile : TerraformableTile
{
    public override void Die()
    {
        // CM.GetComponent<SaveTile>().ChangeState(MapCoords, true);
        
        Caretaker.QueueFree();
        base.Die();
    }
}