using Godot;
using System;
using TerraEngineer;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.objects;
using TerraEngineer.game;
using TerraEngineer.game.ui;

public partial class Teleporter : Entity, IInteractable
{
    public delegate void TeleporterUsedEventHandler();
    public event TeleporterUsedEventHandler TeleporterUsed;
    
    public bool InteractionBlocked { get; set; }

    public override void _Ready()
    {
        GuiMediator gui = GetNode<GuiMediator>(Names.NodePaths.GuiMediator);
        TeleporterUsed += gui.OnTeleporterUsed;
    }

    public void OnInteracted() => TeleporterUsed?.Invoke();
}
