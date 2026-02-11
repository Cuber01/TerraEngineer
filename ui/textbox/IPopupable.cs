using Godot;
using TerraEngineer.game;

namespace TerraEngineer.ui.textbox;

public interface IPopupable
{
    public Controller Controller { get; set;  }

    public void SetupControls();
    public void Display();
    public void Close();
}