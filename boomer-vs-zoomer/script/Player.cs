using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public class Player : KinematicBody
{
	enum PlayerStates
	{
		IDLE,
		WALK,
		JUMP_UP,
		JUMP_FALL,
		ATTACK_1,
		ATTACK_RNG,
		HURT,
		DYING
	}

	//Node references
	AnimatedSprite3D animation;
	Spatial pivot;
	Area attackHitbox;
	AudioStreamPlayer2D playerHit;
	AudioStreamPlayer2D gameOver;
	//

	//Parameters
	float movementSpeed = 600f;
	int maxHealth = 100;
	int currentHealth = 100;
	int attackDamage = 10;
	//

	StateMachine<PlayerStates> stateMachine;
	Vector3 translate = Vector3.Zero;
	float delta;

	bool invulnerable = false;
	bool dead = false;
	bool canHit = true;

	public override void _Ready()
	{
		animation = GetNode<AnimatedSprite3D>("Pivot/Animation");
		pivot = GetNode<Spatial>("Pivot");
		attackHitbox = GetNode<Area>("Pivot/AttackHitbox");
		playerHit = GetNode<AudioStreamPlayer2D>("PlayerHit");
		gameOver =  GetNode<AudioStreamPlayer2D>("GameOver");

		stateMachine = new StateMachine<PlayerStates>(animation);
		stateMachine.AddState(PlayerStates.IDLE, "idle", null, () => UpdateIdle());
		stateMachine.AddState(PlayerStates.WALK, "walk", null, () => UpdateMovement());
		stateMachine.AddState(PlayerStates.ATTACK_1, "attack", () => OnEnterAttack(), () => UpdateAttack());
		stateMachine.AddState(PlayerStates.HURT, "hurt", null, () => UpdateHurt());
		stateMachine.AddState(PlayerStates.DYING, "dead", null, () => UpdateDying());

		animation.Connect("animation_finished", this, "OnAnimationFinished");
	}
	
	public override void _Process(float delta)
	{
		translate = Vector3.Zero;
		this.delta = delta;

		stateMachine.UpdateCurrentState();

		if(translate.Length() > 0.01f)
			MoveAndSlide(translate);

		if(translate.x > 0.01f)
			pivot.RotationDegrees = new Vector3(0, 0, 0);

		else if(translate.x < -0.01f)
			pivot.RotationDegrees = new Vector3(0, 180, 0);
	}

	//States
	private void UpdateIdle() 
	{
		if(VerifyStartAttack())
			return;

		VerifyMovement();

		if(translate.Length() > 0.01f)
			stateMachine.ChangeState(PlayerStates.WALK);
	}

	private void UpdateMovement()
	{
		if(VerifyStartAttack())
			return;

		VerifyMovement();

		if(translate.Length() < 0.01f)
			stateMachine.ChangeState(PlayerStates.IDLE);
	}

	private void OnEnterAttack()
	{
		canHit = true;
	}

	private void UpdateAttack()
	{
		if(canHit && animation.Frame == 3)
		{
			canHit = false;
			List<Node> enemies = attackHitbox.GetOverlappingBodies().Cast<Node>().Where(x => x.IsInGroup("Enemy")).ToList();
			enemies.ForEach(x => {
                if(x.HasMethod("HurtEnemy"))
                {
				    x.Call("HurtEnemy", attackDamage);
                }
			});
		}
	}

	private void UpdateHurt()
	{

	}

	private void UpdateDying()
	{
		if(dead)
			animation.Opacity -= 1f * delta;

		if(animation.Opacity <= 0f){
			gameOver.Play();
			EmitSignal("TriggerGameOver");	
		}
	}

	//Behaviors
	private void VerifyMovement()
	{
		Vector3 moveDirection = new Vector3();

		if(Input.IsActionPressed("move_up"))
			moveDirection.z -= 1;
		if(Input.IsActionPressed("move_down"))
			moveDirection.z += 1;
		if(Input.IsActionPressed("move_left"))
			moveDirection.x -= 1;
		if(Input.IsActionPressed("move_right"))
			moveDirection.x += 1;

		translate += moveDirection.Normalized() * movementSpeed * delta;
	}

	private bool VerifyStartAttack()
	{
		if(Input.IsActionJustPressed("attack"))
		{
			stateMachine.ChangeState(PlayerStates.ATTACK_1);
			return true;
		}

		return false;
	}

	public void OnAnimationFinished()
	{
		switch(animation.Animation)
		{
			case "attack":
				stateMachine.ChangeState(PlayerStates.IDLE);
				break;
			case "hurt":
				invulnerable = false;
				stateMachine.ChangeState(PlayerStates.IDLE);
				break;
			case "dead":
				dead = true;
				break;
			default:
				break;
		}
	}

	public void HitEnemy()
	{

	}

	public void Hurt(int damage)
	{
		invulnerable = true;

		currentHealth -= damage;
		if(currentHealth <= 0)
		{
			stateMachine.ChangeState(PlayerStates.DYING);
		}
		else
			stateMachine.ChangeState(PlayerStates.HURT);
			
		playerHit.Play();
	}

	[Signal]
	public delegate void TriggerGameOver();
}
