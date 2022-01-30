extends Node

var areas_total = 3
var current_area = 1
var area_size = 8

func _process(delta):
	if $"../HUD/EnemyHealthBar".value <= 0:
		$"../HUD/EnemyHealthBar".modulate.a -= 1 * delta
		$"../HUD/EnemyName".modulate.a -= 1 * delta
	else:
		$"../HUD/EnemyHealthBar".modulate.a = 1
		$"../HUD/EnemyName".modulate.a = 1

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
<<<<<<< HEAD


func _on_EndStage_body_entered(body):
	if(body.is_in_group("Player")):
		get_tree().change_scene("res://scenes/GameOver.tscn")
	pass # Replace with function body.
=======
	
func _on_Player_TriggerPlayerHudUpdate(current_life, max_life):
	$"../HUD/PlayerHealthBar".value = current_life
	
func _on_Enemy_TriggerEnemyHudUpdate(name, life):
	$"../HUD/EnemyName".text = name
	$"../HUD/EnemyHealthBar".value = life
>>>>>>> 64d652a (HUD added)
