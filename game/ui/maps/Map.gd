extends Control

@export var textLabel: RichTextLabel;

# The size of the window in cells.
var SIZE: Vector2i

# MapView object that draws cells.
var map_view: MapView

# The player location node from MetSys.add_player_location()
var player_location: Node2D

# The offset for drawing delta vector, updated with map panning.
var offset: Vector2i

func _ready() -> void:
	# Cellular size is total size divided by cell size + shared borders.
	SIZE = size / (MetSys.getCellSizeOffset())
	map_view = MetSys.make_map_view(self, -SIZE / 2, SIZE, 0)
	
	player_location = MetSys.add_player_location(self)


# TODO this needs to be implemented alongisde our controller input system isntead of this
func _input(event: InputEvent) -> void:
	if event is InputEventKey:
		if event.pressed:
			if event.keycode == KEY_M:
				visible = not visible
								
				if visible:
					#get_tree().paused = true
					update_offset(Vector3i.ZERO)
				#else:
					#get_tree().paused = false
			elif visible:
				# Move with arrow keys.
				var move_offset: Vector2i
				if event.keycode == KEY_LEFT:
					move_offset = Vector2i.LEFT
				elif event.keycode == KEY_RIGHT:
					move_offset = Vector2i.RIGHT
				elif event.keycode == KEY_UP:
					move_offset = Vector2i.UP
				elif event.keycode == KEY_DOWN:
					move_offset = Vector2i.DOWN
				
				map_view.move(move_offset)
				update_offset(Vector3i(move_offset.x, move_offset.y, 0))
				
				
func update_offset(extra_offset: Vector3i):
	offset = (MetSys.get_current_flat_coords() - SIZE / 2)
	player_location.offset = -Vector2(offset) * (MetSys.getCellSizeOffset())  
	map_view.move_to(Vector3i(offset.x, offset.y, MetSys.current_layer))
	textLabel.text = get_biome_name(MetSys.last_player_position)
	map_view.update_all()

func get_biome_name(cell_coords: Vector3i) -> String:
	var groups_for_cell: PackedInt32Array = MetSys.map_data.group_cache.get(cell_coords, PackedInt32Array())
	if( len(groups_for_cell) > 0 ):
		return MetSys.map_data.group_names[groups_for_cell[0]]
	else:
		return "Biome name not found"
	# TODO cell group layers can be used for additional markers per cell, 
	# for example it could write "Mushroom Caves, Boss Room, Uncollected item"
