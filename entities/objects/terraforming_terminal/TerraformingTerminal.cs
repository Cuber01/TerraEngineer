using DialogueManagerRuntime;
using Godot;
using System;
using TENamespace.ui.dialogue_box;
using TerraEngineer.entities.mobs;

namespace TerraEngineer.entities.objects.terraforming_terminal;

public partial class TerraformingTerminal : Entity, IInteractable
{
    [Export] private Resource dialogueResource;
    [Export] private AnimationPlayer animationPlayer;
    
    public bool InteractionBlocked { get; set; }
    
    private Player player;
    private DialogueBalloon balloonTemplate;
    private TerraformableCaretaker caretaker;

    public override void _Ready()
    {
        balloonTemplate = GetNode<DialogueBalloon>(Names.NodePaths.DialogueBalloon);
        player = GetNode<Player>(Names.NodePaths.Player);
        caretaker = GetParent<TerraformableCaretaker>();
        caretaker.Terraformed += updateSprite;
    }
    
    public void OnInteracted()
    {
        DialogueManager.DialogueEnded += executeChosenOption;
        balloonTemplate.PlayDialogue(dialogueResource, Names.Other.Start);
        player.Controller.SwitchControl(balloonTemplate.Controller);
    }
    
    private void executeChosenOption(Resource dialogueResource)
    {
        caretaker.Terraform((Biomes)GlobalDialoguesState.Instance.PuzzleTerraformingRoom_Biome);
        DialogueManager.DialogueEnded -= executeChosenOption;
    }

    private void updateSprite(Biomes newBiome)
    {
        switch (newBiome)
        {
            case Biomes.Forest:
                animationPlayer.Play("green");
                break;
            case Biomes.Ice:
                animationPlayer.Play("blue");
                break;
            case Biomes.Mushroom:
                animationPlayer.Play("red");
                break;
            case Biomes.Desert:
                animationPlayer.Play("yellow");
                break;
        }
    }
}
