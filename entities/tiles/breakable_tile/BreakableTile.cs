using Godot;
using TENamespace.basic.save_tile;
using TerraEngineer.entities.mobs;

namespace TerraEngineer.entities.tiles;

public partial class BreakableTile : Tile
{
    public override void Die()
    {
        CM.GetComponent<SaveTile>().ChangeState(MapCoords, false);
        
        base.Die();
    }
}