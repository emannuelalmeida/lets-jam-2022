extends Node

var areas_total = 3
var current_area = 1
var area_size = 8

func _move_camera_to_next_scene():
		var camera = get_node("../CameraPivot/Camera")
		if (current_area == areas_total):
			pass
		else:
			current_area += 1
			for i in range(area_size):
				camera._move_camera_next_area(1)
				yield(get_tree().create_timer(0.06), "timeout")

func _on_Player_TriggerGameOver():
	get_tree().change_scene("res://scenes/GameOver.tscn")


func _on_EndStage_body_entered(body):
	if(body.is_in_group("Player")):
		get_tree().change_scene("res://scenes/GameOver.tscn")
	pass # Replace with function body.
