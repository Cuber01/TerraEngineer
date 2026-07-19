namespace TerraEngineer.game.ui;

public interface IUserInterface
{
    public bool IsOpen { get; set; }
    
    public void Open();
    public void Close();
}