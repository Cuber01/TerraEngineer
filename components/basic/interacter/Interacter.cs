using Godot;
using System;
using TENamespace;
using TerraEngineer;
using TerraEngineer.entities.objects;
using TerraEngineer.game;
using TerraEngineer.game.ui;

public partial class Interacter : Component
{
    private Player playerActor;
    private InputContext interactContext;
    private bool pushedContext = false;
    
    public override void Init(Node2D actor)
    {
        base.Init(actor);
        if (actor is Player player)
        {
            playerActor = player;
        }
        else
        {
            throw new Exception("Interacter component requires Player actor.");
        }
        
        interactContext = new InputContext();
    }
    
    private IInteractable GetInteractable(Node2D area)
    {
        if (area is IInteractable interactable) return interactable;
        if (area.GetParent() is IInteractable parentInteractable) return parentInteractable;
        return null;
    }

    private void onInteractableAreaEntered(Area2D area)
    {
        IInteractable interactable = GetInteractable(area);
        if (interactable is { InteractionBlocked: false })
        {
            interactContext.AddAction(Names.Actions.Attack, interactable.OnInteracted);
            InputStackManager.Push(interactContext);
            pushedContext = true;
        }
    }
    
    private void onInteractableAreaExited(Area2D area)
    {
        if(pushedContext)
        {
            InputStackManager.Pop();
            pushedContext = false;
        }
    }
}
