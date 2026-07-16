using Godot;
using System;
using System.Numerics;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.objects;
using Vector2 = Godot.Vector2;

namespace TerraEngineer.entities;

public interface ITerraformable
{
    [Export] public Biomes MyBiome { get; set; }
    public bool Active { get; set; }
    public TerraformableCaretaker Caretaker { get; set; }

    public void Setup(TerraformableCaretaker caretaker)
    {
        Caretaker = caretaker;
    }

    public void Enable();
    public void Disable();
}