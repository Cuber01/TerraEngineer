namespace TerraEngineer.ui.player_hud;

public interface IConnectable<T>
{
    public void Connect(T actor);
    public void Disconnect(T actor);
}