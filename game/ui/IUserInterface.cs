namespace TerraEngineer.game.ui;

public interface IUserInterface
{
    public delegate void ClosedInternallyEventHandler(IUserInterface me);
    public event ClosedInternallyEventHandler ClosedInternally;
    
    public bool IsOpen { get; set; }
    
    public void Open();
    public void Close();
}