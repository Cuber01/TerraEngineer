class_name WorldManager
extends MetSysGame

@export var extPlayer : Node2D

var CurrentLevel: Node
var gameInitialized: bool = false

func _ready():
	start()
	$GUI/PlayerHUD.Connect(player)
	$GUI/MapScreen/Panel.Connect(player)
	$GUI/InventoryScreenStarter.Connect(player)

func on_room_loaded(level: Node):
	CurrentLevel = level
	$LevelPreparer.Prepare(CurrentLevel)
	
	if (!gameInitialized):
		player.global_position = SaveData.StringToVec(SaveData.ReadValue("savepoint_data", "position"))
		player.ActivateInventory()
		gameInitialized = true
	
	MetSys.get_current_room_instance().adjust_camera_limits(player.get_node("Camera2D"))
	player.onRoomTransitionFinished()

func start():
	MetSys.reset_state()
	MetSys.set_save_data()
	extPlayer.global_position = Vector2(11, 131)
	set_player(extPlayer)
	player.Freeze()
	add_module("RoomTransitions.gd")
	room_loaded.connect(on_room_loaded)
	MetSys.room_changed.connect(Callable(player, "onRoomEntered"))

	var levelName = SaveData.ReadValue("savepoint_data", "level")
	await load_room("res://assets/levels/" + levelName + ".tscn")
