using System.Diagnostics;
using Godot;
using System;

public class Enemy : KinematicBody
{
    enum EnemyStates {
        IDLE,
        FOLLOW,
        ATTACK,
        ATTACK2,
        ATTACK3,
        HURT,
        DYING,
    }

    //Node references
    AnimatedSprite3D animation;
    KinematicBody player;
    Area attackHitbox;
    Position3D hurtAnimationSource;
    Spatial pivot;
    //===============

    StateMachine<EnemyStates> stateMachine;
    Vector3 translate;
    float deltaRef = 0f;

    //Parameters
    [Export]
    float MoveSpeed {get; set;} = 4f;

    [Export]
    float attackCooldownSeconds = 3f;

    [Export]
    int attackDamage = 5;

    [Export]
    int health = 100;

    bool playerInAttackRange = false;
    bool canAttack = true;
    bool canHit = true;
    bool invulnerable = false;
    bool dead = false;

    PackedScene hurtAnimation;

    [Signal]
    delegate void HurtPlayer(int damage);

    public override void _Ready()
    {
        animation = GetNode<AnimatedSprite3D>("Pivot/Animation");
        attackHitbox = GetNode<Area>("Pivot/AttackHitbox");
        pivot = GetNode<Spatial>("Pivot");

        hurtAnimationSource = GetNode<Position3D>("Pivot/AttackHitbox/CollisionShape/HurtAnimationSource");

        stateMachine = new StateMachine<EnemyStates>(animation);

        hurtAnimation = ResourceLoader.Load<PackedScene>("res://scenes/Objects/Hurt_Animation.tscn");

        stateMachine.AddState(EnemyStates.IDLE, "idle", null, () => UpdateIdle());
        stateMachine.AddState(EnemyStates.FOLLOW, "run", null, () => UpdateFollow());
        stateMachine.AddState(EnemyStates.ATTACK, "attack", () => OnEnterAttack(), () => UpdateAttack());
        stateMachine.AddState(EnemyStates.ATTACK2, "attack2", () => OnEnterAttack(), () => UpdateAttack());
        stateMachine.AddState(EnemyStates.ATTACK3, "attack3", () => OnEnterAttack(), () => UpdateAttack());
        stateMachine.AddState(EnemyStates.HURT, "hurt", null, null);
        stateMachine.AddState(EnemyStates.DYING, "dying", () => OnEnterDying(), () => UpdateDying(deltaRef));

        animation.Connect("animation_finished", this, "OnAnimationFinished");
    }

    public override void _Process(float delta)
    {
        deltaRef = delta;

        translate = Vector3.Zero;

        stateMachine.UpdateCurrentState();

        this.MoveAndSlide(translate);

        if(translate.x > 0.01f)
            pivot.RotationDegrees = new Vector3(0, 0, 0);

        else if(translate.x < -0.01f)
            pivot.RotationDegrees = new Vector3(0, 180, 0);
    }

#region States

    void UpdateIdle() 
    {
        if(!playerInAttackRange && player != null)
            stateMachine.ChangeState(EnemyStates.FOLLOW);
        
        if(playerInAttackRange && canAttack)
        {
            float atkRng = GD.Randf();
            if(atkRng < 0.5f)
                stateMachine.ChangeState(EnemyStates.ATTACK);
            else if(atkRng < 0.8f)
                stateMachine.ChangeState(EnemyStates.ATTACK2);
            else 
                stateMachine.ChangeState(EnemyStates.ATTACK3);
        }
    }

    void UpdateFollow()
    {
        if(player == null)
        {
            stateMachine.ChangeState(EnemyStates.IDLE);
            return;
        }

        if(playerInAttackRange && !canAttack)
        {
            stateMachine.ChangeState(EnemyStates.IDLE);
            return;
        }

        if(playerInAttackRange && canAttack)
        {
            stateMachine.ChangeState(EnemyStates.ATTACK);
            return;
        }

        var direction = player.Transform.origin - this.Transform.origin;
        direction.y = 0;
        direction = direction.Normalized();
        translate = direction * MoveSpeed;
    }

    void OnEnterAttack()
    {
        canAttack = false;

        var cooldown = (float)GD.RandRange(attackCooldownSeconds -0.75d, attackCooldownSeconds + 0.75d);
        var signal = ToSignal(GetTree().CreateTimer(cooldown, false), "timeout");

        signal.OnCompleted(() => {
            canAttack = true;
            canHit = true;
        });

        attackHitbox.Monitoring = true;
    }

    void UpdateAttack()
    {
        if(animation.Frame == 4 && canHit && attackHitbox.OverlapsBody(player))
        {
            canHit = false;
            OnHitPlayer();
        }
    }

    void OnEnterDying()
    {
        invulnerable = true;
    }

    void UpdateDying(float delta)
    {
        if(dead)
        {
            animation.Opacity -= 1 * delta;

            if(animation.Opacity <= 0)
                QueueFree();
        }
    }

#endregion

    public void OnAnimationFinished()
    {
        switch(animation.Animation)
        {
            case "attack":
            case "attack2":
            case "attack3":
                stateMachine.ChangeState(EnemyStates.IDLE);
                break;
            case "hurt":
                stateMachine.ChangeState(EnemyStates.IDLE);
                break;
            case "dying":
                dead = true;
                break;
            default:
                break;
        }
    }

    private void OnHitPlayer()
    {
        var instance = hurtAnimation.Instance<Spatial>();
        hurtAnimationSource.AddChild(instance);

        player.Call("Hurt", attackDamage);
    }

    public void HurtEnemy(int damage)
    {
        if(invulnerable)
            return;
        
        health -= damage;

        if(health > 0)
            stateMachine.ChangeState(EnemyStates.HURT);
        else
            stateMachine.ChangeState(EnemyStates.DYING);
    }

#region Connections

    private void _on_PlayerRange_body_entered(Node body)
    {
        if(body.IsInGroup("Player"))
        {
            player = body as KinematicBody;
        }
    }

    private void _on_PlayerRange_body_exited(Node body)
    {
        if(body.IsInGroup("Player"))
        {
            player = null;
        }
    }

    private void _on_InAttackRange_body_entered(Node body)
    {
        if(body.IsInGroup("Player"))
        {
            playerInAttackRange = true;
            GD.Print("Player IN range of attack!");
        }
    }

    private void _on_InAttackRange_body_exited(Node body)
    {
        if(body.IsInGroup("Player"))
        {
            playerInAttackRange = false;
            GD.Print("Player OUT of range of attack!");
        }
    }

#endregion
}
