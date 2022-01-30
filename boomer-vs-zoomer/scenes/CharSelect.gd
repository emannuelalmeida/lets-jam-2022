extends Node2D

var selected_char = 0
var num_chars = 4
var unblock = [true, false, false, false]
var file_name = "user://save.dt"

func _ready():
	var save_game = File.new()
	if save_game.file_exists(file_name):
		#unblock = restore_last_save(save_game)
		pass
	else:
		unblock = [true, false, false, false]

func _process(delta):
	if Input.is_action_just_pressed("move_left"):
		selected_char -= 1
	if Input.is_action_just_pressed("move_right"):
		selected_char += 1
	if Input.is_action_just_pressed("attack"):
		pass
		
	if selected_char > num_chars - 1:
		selected_char = num_chars - 1
	elif selected_char < 0:
		selected_char = 0

func update_char_selection():
	pass #Here comes the selection to go to another scene. Decide how to pass that.
	
func restore_last_save(save):
	save.open(file_name, File.READ)
	var unblock = save.get_var()
	
	save.close()
	return unblock
	
	
