extends Node2D

var selected_option = 0
var main_menu = true
var menu_color = Color("fa8842")
var selected_color = Color("7e0305")

var options = ["Node/Start", "Node/Extra", "Node/Credits", "Node/Quit"]

func _process(delta):
	if Input.is_action_just_pressed("move_up"):
		selected_option -= 1
	if Input.is_action_just_pressed("move_down"):
		selected_option += 1
	if Input.is_action_just_pressed("attack"):
		match selected_option:
			0:
				change_char_screen()
			2:
				change_credits_screen()
			3:
				quit_game()
	if selected_option > 3:
		selected_option = 0
	elif selected_option < 0:
		selected_option = 3
	
	update_selected_option_color(selected_option)

func update_selected_option_color(selected_option):
	for i in range(0, 4):
		var option = get_node(options[i])
		if i == selected_option:
			option.add_color_override("default_color", selected_color)
		else:
			option.add_color_override("default_color", menu_color)

func change_char_screen():
	#get_tree().change_scene("res://scenes/Char_select.tscn")
	pass
	
func change_credits_screen():
	get_tree().change_scene("res://scenes/Credits.tscn")

func quit_game():
	get_tree().quit()
