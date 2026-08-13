
namespace TerraEngineer.entities.tiles;

public partial class BreakableTile : TerraformableEntity
{
    public override void Die()
    {
        // CM.GetComponent<SaveTile>().ChangeState(MapCoords, true);
        
        Caretaker.QueueFree();
        base.Die();
    }
}