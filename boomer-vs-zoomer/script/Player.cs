using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public class Player : KinematicBody
{
	public int speed = 14;
	public int dashSpeed = 28;

	private readonly Timer timer = new Timer();

	private InputManager inputManager;
	public Behavior currentState;

	private Dictionary<string, InputAction> actions;
	private Dictionary<string, Behavior> allStates;

	enum PlayerStates
	{
		IDLE,
		WALK,
		DASH,
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
	public Vector3 translate = Vector3.Zero;
	float delta;

	bool invulnerable = false;
	bool dead = false;
	bool canHit = true;

	public override void _Ready()
	{

		Dictionary<string, string> keyMap1 = new Dictionary<string, string>
		{
			["UP"] = "move_up",
			["DOWN"] = "move_down",
			["LEFT"] = "move_left",
			["RIGHT"] = "move_right",
			["JUMP"] = "jump",
			["PUNCH"] = "attack",
			// ["KICK"] = ""
		};

		initCharacter(CharactersLib.char1);
		inputManager = new InputManager(keyMap1, actions.Values);
		currentState = allStates["standing"];

		animation = GetNode<AnimatedSprite3D>("Pivot/Animation");
		pivot = GetNode<Spatial>("Pivot");
		attackHitbox = GetNode<Area>("Pivot/AttackHitbox");
		playerHit = GetNode<AudioStreamPlayer2D>("PlayerHit");
		gameOver =  GetNode<AudioStreamPlayer2D>("GameOver");

		stateMachine = new StateMachine<PlayerStates>(animation);
		stateMachine.AddState(PlayerStates.IDLE, "idle", null, () => UpdateIdle());
		stateMachine.AddState(PlayerStates.WALK, "walk", null, () => UpdateMovement());
		stateMachine.AddState(PlayerStates.DASH, "walk", null,
			() => currentState.updater.Update(this, inputManager.ticks));
		stateMachine.AddState(PlayerStates.ATTACK_1, "attack", () => OnEnterAttack(), () =>
		{
			UpdateAttack();
			currentState.updater.Update(this, inputManager.ticks);
		});
		stateMachine.AddState(PlayerStates.HURT, "hurt", null, () => UpdateHurt());
		stateMachine.AddState(PlayerStates.DYING, "dead", null, () => UpdateDying());

		animation.Connect("animation_finished", this, "OnAnimationFinished");
	}

	public override void _PhysicsProcess(float delta)
	{
		translate = Vector3.Zero;
		this.delta = delta;

		inputManager.update(this);
		stateMachine.UpdateCurrentState();

		if(translate.Length() > 0.01f)
			MoveAndSlide(translate);

		if(translate.x > 0.01f)
			pivot.RotationDegrees = new Vector3(0, 0, 0);

		else if(translate.x < -0.01f)
			pivot.RotationDegrees = new Vector3(0, 180, 0);
	}

	private void initCharacter(Character character)
	{
		actions = character.actions.ToDictionary(
			action => action.name,
			action => action);
		allStates = character.states.ToDictionary(
			state => state.name,
			state => state);
		speed = character.moveSpeed;
		dashSpeed = character.dashSpeed;
	}

	private void ChangeState(string newStateName)
	{
		PlayerStates newState;
		switch (newStateName)
		{
			case "walking":
				newState = PlayerStates.WALK;
				break;
			case "dashing":
				newState = PlayerStates.DASH;
				break;
			case "first-punch":
				newState = PlayerStates.ATTACK_1;
				break;
			case "standing":
			default:
				newState = PlayerStates.IDLE;
				break;
		}

		stateMachine.ChangeState(newState);
		currentState = allStates[newStateName];
	}

	public void submit(ActionEvent actionEvent)
	{
		if (actionEvent.released)
		{
			ChangeState(actionEvent.action.releaseState);
		}
		else
		{
			ChangeState(actionEvent.action.changeState);
		}
	}

	public void submit(EventType eventType)
	{
		if (eventType == EventType.IDLE)
		{
			ChangeState("standing");
		}
	}
	
	//States
	private void UpdateIdle() 
	{
		// if(VerifyStartAttack())
		// 	return;
		//
		// VerifyMovement();
		//
		// if(translate.Length() > 0.01f)
		// 	stateMachine.ChangeState(PlayerStates.WALK);
	}

	private void UpdateMovement()
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
			GD.Print($"Enemies: {enemies.Count}");
			enemies.ForEach(x => {
				x.Call("HurtEnemy", attackDamage);
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
	private bool VerifyStartAttack()
	{
		// if(Input.IsActionJustPressed("attack"))
		// {
		// 	stateMachine.ChangeState(PlayerStates.ATTACK_1);
		// 	return true;
		// }

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

	public bool countdownReset(int ticksCount)
	{
		return timer.countdown(3, ticksCount);
	}

}

public class Character
{
	public int moveSpeed;
	public int dashSpeed;
	public List<Behavior> states = new List<Behavior>();
	public List<InputAction> actions = new List<InputAction>();
}

public class CharactersLib
{
	public static Character char1 = new Character()
	{
		moveSpeed = 14,
		dashSpeed = 28,
		states = new List<Behavior>
		{
			new Behavior("ground"),
			new Behavior("standing")
			{
				updater = new Updater<Player>.Move(0, 0),
				flags = new List<string> { "ground" }
			},
			new Behavior("walking")
			{
				updater = new Updater<Player>.PlayerMove(),
				flags = new List<string> { "ground" }
			},
			new Behavior("dashing")
			{
				updater = new Updater<Player>.PlayerDash(),
				flags = new List<string> { "ground" }
			},
			new Behavior("jumping"),
			new Behavior("air"),
			new Behavior("attacking"),
			new Behavior("first-punch")
			{
				updater = new Updater<Player>.Composite<Player>(
					new Updater<Player>.Attack
					{
						attackFrame = 3,
						attackDamage = 10,
						collisionBox = new AttackCollisionBox(50,-25,0,100,50,1)
					},
					new Updater<Player>.AutoReset(25)),
				flags = new List<string> { "ground", "attacking" }
			},
			new Behavior("second-punch")
			{
				flags = new List<string> { "ground", "attacking" }
			},
		},
		actions = new List<InputAction> {
			new InputAction
			{
				name = "Idle",
				eventType = EventType.IDLE,
				changeState = "standing"
			},
			new InputAction
			{
				name = "WalkUp",
				eventType = EventType.INPUT,
				sequence = new List<ActionSequence> { new ActionSequence.Press("UP") },
				priority = 0,
				startStates = new List<string> { "standing", "walking" },
				changeState = "walking",
				releaseState = "standing"
			},
			new InputAction
			{
				name = "WalkDown",
				eventType = EventType.INPUT,
				sequence = new List<ActionSequence> { new ActionSequence.Press("DOWN") },
				priority = 0,
				startStates = new List<string> { "standing", "walking" },
				changeState = "walking",
				releaseState = "standing"
			},
			new InputAction
			{
				name = "WalkLeft",
				eventType = EventType.INPUT,
				sequence = new List<ActionSequence> { new ActionSequence.Press("LEFT") },
				priority = 0,
				startStates = new List<string> { "standing", "walking" },
				changeState = "walking",
				releaseState = "standing"
			},
			new InputAction
			{
				name = "WalkRight",
				eventType = EventType.INPUT,
				sequence = new List<ActionSequence> { new ActionSequence.Press("RIGHT") },
				priority = 0,
				startStates = new List<string> { "standing", "walking" },
				changeState = "walking",
				releaseState = "standing"
			},
			new InputAction
			{
				name = "StartDashLeft",
				eventType = EventType.INPUT,
				sequence = new List<ActionSequence>
				{
					new ActionSequence.Release("LEFT"),
					new ActionSequence.Press("LEFT", uint.MaxValue),
					new ActionSequence.Release("LEFT"),
					new ActionSequence.Press("LEFT")
				},
				priority = 2,
				startStates = new List<string> { "standing", "walking" },
				changeState = "dashing",
				releaseState = "standing"
			},
			new InputAction
			{
				name = "StartDashRight",
				eventType = EventType.INPUT,
				sequence = new List<ActionSequence>
				{
					new ActionSequence.Release("RIGHT"),
					new ActionSequence.Press("RIGHT", uint.MaxValue),
					new ActionSequence.Release("RIGHT"),
					new ActionSequence.Press("RIGHT")
				},
				priority = 1,
				startStates = new List<string> { "standing", "walking" },
				changeState = "dashing",
				releaseState = "standing"
			},
			new InputAction
			{
				name = "DashUp",
				eventType = EventType.INPUT,
				sequence = new List<ActionSequence>
				{
					new ActionSequence.Press("UP")
				},
				priority = 1,
				startStates = new List<string> { "dashing" },
				changeState = "dashing",
				releaseState = "standing"
			},
			new InputAction
			{
				name = "DashDown",
				eventType = EventType.INPUT,
				sequence = new List<ActionSequence>
				{
					new ActionSequence.Press("DOWN")
				},
				priority = 1,
				startStates = new List<string> { "dashing" },
				changeState = "dashing",
				releaseState = "standing"
			},
			new InputAction
			{
				name = "DashLeft",
				eventType = EventType.INPUT,
				sequence = new List<ActionSequence>
				{
					new ActionSequence.Press("LEFT")
				},
				priority = 1,
				startStates = new List<string> { "dashing" },
				changeState = "dashing",
				releaseState = "standing"
			},
			new InputAction
			{
				name = "DashRight",
				eventType = EventType.INPUT,
				sequence = new List<ActionSequence>
				{
					new ActionSequence.Press("RIGHT")
				},
				priority = 1,
				startStates = new List<string> { "dashing" },
				changeState = "dashing",
				releaseState = "standing"
			},
			new InputAction
			{
				name = "FirstPunch",
				eventType = EventType.INPUT,
				sequence = new List<ActionSequence>
				{
					new ActionSequence.Release("PUNCH"),
					new ActionSequence.Press("PUNCH", uint.MaxValue)
				},
				priority = 10,
				startStates = new List<string> { "standing", "walking" },
				changeState = "first-punch"
			}
		}
	};
}
