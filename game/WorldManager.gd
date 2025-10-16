class_name WorldManager
extends MetSysGame

@export var extPlayer : Node2D;

func _ready():
	start();

func start():
	MetSys.reset_state();
	MetSys.set_save_data();
	extPlayer.global_position = Vector2(50, 50);
	set_player(extPlayer);
	add_module("RoomTransitions.gd");
	
	await load_room("res://levels/Lobby.tscn");
	
	MetSys.get_current_room_instance().adjust_camera_limits(player.get_node("Camera2D"))
