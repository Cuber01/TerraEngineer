namespace TerraEngineer.entities.mobs.creatures;

public interface ISibling
{
    protected bool SiblingDied { get; set; }
    
    public void ConnectToSibling(Entity sibling)
    {
        sibling.Died += () =>
        {
            SiblingDied = true;
        };
    }
}