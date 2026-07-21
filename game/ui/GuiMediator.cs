using Godot;
using TerraEngineer.game.ui.inventory;
using TerraEngineer.ui.maps;

namespace TerraEngineer.game.ui;

public partial class GuiMediator : Node
{
    private InputContext uiGeneralContext;
    private InputContext uiOpenContext;

    private IUserInterface currentUI;
    
    private IUserInterface map;
    private IUserInterface inventory;
    private IUserInterface hud;
    
    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        
        map = GetNode<IUserInterface>(Names.NodePaths.Map);
        inventory = GetNode<IUserInterface>(Names.NodePaths.Inventory);
        hud = GetNode<IUserInterface>(Names.NodePaths.PlayerHUD);
        
        inventory.ClosedInternally += close;
        map.ClosedInternally += close;
        hud.ClosedInternally += close;
        
        uiGeneralContext = new InputContext();
        uiOpenContext = new InputContext();
        
        uiGeneralContext.AddAction(Names.Actions.OpenMap, () => closeOpenAction(map));
        uiGeneralContext.AddAction(Names.Actions.OpenInventory, () => closeOpenAction(inventory));
        
        uiOpenContext.AddAction(Names.Actions.Quit, () => closeOpenAction(currentUI) );
        
        InputStackManager.Push(uiGeneralContext);
    }
    
    private void closeOpenAction(IUserInterface ui)
    {
        if (ui.IsOpen)
        {
            close(ui);
        }
        else
        {
            open(ui);
        }
    }

    private void open(IUserInterface ui)
    {
        ui.Open();
        currentUI = ui;
        InputStackManager.Push(uiOpenContext);
            
            
        if (ui is Map || ui is InventoryScreenStarter)
        {
            hud.Close();
        }
    }

    private void close(IUserInterface ui)
    {
        ui.Close();
        InputStackManager.Pop();
        currentUI = null;

        if (ui is Map || ui is InventoryScreenStarter)
        {
            hud.Open();
        }
    }

    public void OnTeleporterUsed()
    {
        InputStackManager.Push(uiOpenContext);
        ((Map)map).OpenForTeleporter();
        currentUI = map;
        hud.Close();
    }
    
}