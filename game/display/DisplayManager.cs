using Godot;
using TerraEngineer.game.ui;

namespace TerraEngineer.game.display;

public partial class DisplayManager : Node2D
{
    private InputContext displayContext = new();

    public static int Scale = 4;
    public static readonly Vector2I BaseSize = new Vector2I(320, 180);
    public static Vector2I WindowedSize = BaseSize*Scale;
    
    public override void _Ready()
    {
        displayContext.AddAction("f11", toggleFullscreen);
        InputStackManager.Push(displayContext);
    }
    
    private void toggleFullscreen()
    {
        var currentMode = DisplayServer.WindowGetMode();

        if (currentMode == DisplayServer.WindowMode.ExclusiveFullscreen ||
            currentMode == DisplayServer.WindowMode.Fullscreen)
        {
            var window = GetWindow();

            window.Mode = Window.ModeEnum.Windowed;
            window.Borderless = false;

            window.Size = WindowedSize;

            // center
            Vector2I screenSize = DisplayServer.ScreenGetSize();
            window.Position = (screenSize - WindowedSize) / 2;
        }
        else
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen); // ExclusiveFullscreen is better than fullscreen
            DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, true);
        }
    }
    
}