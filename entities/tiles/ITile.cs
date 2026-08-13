using Godot;

namespace TerraEngineer.entities.tiles;

public interface ITile
{
    public Vector2I MapCoords { get; set; }
}