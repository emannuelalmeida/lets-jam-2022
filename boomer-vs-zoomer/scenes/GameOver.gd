extends Node2D

func _process(delta):
	if Input.is_action_just_pressed("attack"):
		get_tree().change_scene("res://scenes/Menu.tscn")
