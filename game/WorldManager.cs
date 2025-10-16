using Godot;
using System;


public partial class WorldManager : Node2D
{
    [Export] private Node2D metSysGame;
    [Export] private Node2D player;
    private Node metSysGlobal;
    
    public override void _Ready()
    {
        metSysGlobal = GetNode<Node>("/root/MetSys");
        start();
    }
    
    private void start()
    {
        metSysGlobal.Call("reset_state");
        metSysGlobal.Call("set_save_data");
        metSysGame.Call("set_player", player);
        metSysGame.Call("add_module", "RoomTransitions.gd");
        
        metSysGame.Call("load_room", "res://levels/Lobby.tscn");
        
        
    }
}
