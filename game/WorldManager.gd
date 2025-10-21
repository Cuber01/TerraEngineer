class_name WorldManager
extends MetSysGame

@export var extPlayer : Node2D;

var CurrentLevel: Node;

func _ready():
	start();

func on_room_loaded(level: Node):
	CurrentLevel = level;

func start():
	MetSys.reset_state();
	MetSys.set_save_data();
	extPlayer.global_position = Vector2(50, 50);
	set_player(extPlayer);
	add_module("RoomTransitions.gd");
	connect("room_loaded", on_room_loaded);
	await load_room("res://levels/Lobby.tscn");

	MetSys.get_current_room_instance().adjust_camera_limits(player.get_node("Camera2D"))
