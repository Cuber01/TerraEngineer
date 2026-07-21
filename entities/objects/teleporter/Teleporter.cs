using Godot;
using System;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.objects;
using TerraEngineer.game;

public partial class Teleporter : Entity, IInteractable
{
    public bool InteractionBlocked { get; set; }
    
    public void OnInteracted()
    {
        throw new NotImplementedException();
    }
}
