extends Camera

func _move_camera_next_area(num_pix):
	.translate(Vector3(num_pix, 0, 0))
