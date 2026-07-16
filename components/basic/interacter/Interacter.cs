using Godot;
using System;
using TENamespace;
using TerraEngineer;
using TerraEngineer.entities.objects;

public partial class Interacter : Component
{
    private Player playerActor;
    
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
            playerActor.Controller.AddOverride(Names.Actions.Attack, interactable.OnInteracted);
        }
    }
    
    private void onInteractableAreaExited(Area2D area)
    {
        IInteractable interactable = GetInteractable(area);
        if (interactable is { InteractionBlocked: false })
        {
            playerActor.Controller.RemoveOverride(Names.Actions.Attack);
        }
    }
}
