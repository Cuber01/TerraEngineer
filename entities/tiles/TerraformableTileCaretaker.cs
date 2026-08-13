using Godot;

namespace TerraEngineer.entities.tiles;

public partial class TerraformableTileCaretaker : TerraformableCaretaker, ITile
{
    public Vector2I MapCoords { get; set; }

}