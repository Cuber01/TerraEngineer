using Godot;
using System;
using TerraEngineer;

public partial class SecretRoom : Area2D
{
    [Export] private TileMapLayer fakeTileMap;
    private bool disappear = false;
    private float disappearingSpeed = 10;
    
    private void onPlayerEntered(Node2D _)
        => disappear = true;

    public override void _Process(double delta)
    {
        if (disappear)
        {
            fakeTileMap.Modulate =
                new Color(fakeTileMap.Modulate, MathT.Lerp(fakeTileMap.Modulate.A, 0.0f, disappearingSpeed, (float)delta));
        }
    }
}
