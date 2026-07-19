using TerraEngineer.game;

namespace TerraEngineer.ui.textbox;

public interface IPopupable
{
    public InputContext InputContext { get; set;  }

    public void SetupControls();
    public void Close();
}