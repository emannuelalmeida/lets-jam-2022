extends KinematicBody

const jump_speed = 23
const speed = 14

var velocity = Vector3.ZERO
onready var gravityDir = ProjectSettings.get_setting("physics/3d/default_gravity_vector")
onready var gravityVal = ProjectSettings.get_setting("physics/3d/default_gravity")
onready var gravity = ProjectSettings.get_setting("physics/3d/default_gravity") * ProjectSettings.get_setting("physics/3d/default_gravity_vector")

var is_jumping = false
var vertical_velocity = 0.0


func _physics_process(delta):
	# We create a local variable to store the input direction.
	var direction = Vector3.ZERO
	if is_jumping:
		vertical_velocity -= gravityVal * delta
	else:
		vertical_velocity = 0

	# We check for each move input and update the direction accordingly.
	if Input.is_action_pressed("move_right"):
		direction.x += 1
	if Input.is_action_pressed("move_left"):
		direction.x -= 1
	if Input.is_action_pressed("move_down"):
		direction.z += 1
	if Input.is_action_pressed("move_up"):
		direction.z -= 1
	
	if Input.is_action_pressed("jump") and not is_jumping:
		vertical_velocity = jump_speed
		is_jumping = true
	
	if(abs(direction.x) > 0):
		$Pivot/Animation.play("walk")
	else:
		$Pivot/Animation.play("idle")
	
	if abs(direction.x) > 0.01:
		var lookdirection = Vector3()
		direction = direction.normalized();
		lookdirection.x = direction.x
		$Pivot/Animation.flip_h = direction.x < -0.01;
		
	
	velocity.x = direction.x * speed
	velocity.z = direction.z * speed
	velocity.y = vertical_velocity
	# Moving the character
	velocity = move_and_slide(velocity, -gravityDir)
	
	if is_on_floor():
		direction.y = 0.0
		is_jumping = false
	
