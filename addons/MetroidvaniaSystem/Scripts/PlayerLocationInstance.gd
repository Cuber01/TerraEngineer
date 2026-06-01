extends Node2D

var offset: Vector2
var exact: bool

func _ready() -> void:
	exact = MetSys.settings.theme.show_exact_player_location
	z_index = 5

func _notification(what: int) -> void:
	if what == NOTIFICATION_VISIBILITY_CHANGED:
		if is_visible_in_tree():
			process_mode = Node.PROCESS_MODE_INHERIT
		else:
			process_mode = Node.PROCESS_MODE_DISABLED

func _process(delta: float) -> void:
	var last_player_position_2d := Vector2(MetSys.last_player_position.x, MetSys.last_player_position.y)
	var draw_size := MetSys.CELL_SIZE + Vector2.ONE
	var player_position := last_player_position_2d * draw_size + draw_size * 0.5
	if exact:
		player_position += (MetSys.exact_player_position / MetSys.settings.in_game_cell_size).posmod(1) * draw_size - draw_size * 0.5

	# Snap to integer pixels for pixel-perfect rendering and nudge by (-1,-1)
	position = (player_position + offset).round() + Vector2(-1, -1)
