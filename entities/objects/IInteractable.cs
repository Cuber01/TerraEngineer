namespace TerraEngineer.entities.objects;

public interface IInteractable
{
    public bool InteractionBlocked { get; set; }
    
    public void OnInteracted();
}