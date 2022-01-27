using System.Diagnostics;
using Godot;
using System;

public class Enemy : KinematicBody
{
    enum EnemyStates {
        IDLE,
        FOLLOW,
        ATTACK,
        HURT,
        DYING,
    }

    StateMachine<EnemyStates> stateMachine;
    AnimatedSprite3D animation;
    Vector3 translate;
    KinematicBody player;

    public float moveSpeed = 4f;

    public override void _Ready()
    {
        animation = GetNode<AnimatedSprite3D>("Pivot/Animation");
        stateMachine = new StateMachine<EnemyStates>(animation);

        stateMachine.AddState(EnemyStates.IDLE, "idle", null, () => UpdateIdle());
        stateMachine.AddState(EnemyStates.FOLLOW, "run", null, () => UpdateFollow());
    }

    public override void _Process(float delta)
    {
        translate = Vector3.Zero;

        stateMachine.UpdateCurrentState();

        this.MoveAndSlide(translate);

        if(translate.x > 0.01f)
            animation.FlipH = false;

        else if(translate.x < -0.01f)
            animation.FlipH = true;
    }

    void UpdateIdle() 
    {
        if(player != null)
            stateMachine.ChangeState(EnemyStates.FOLLOW);
    }

    void UpdateFollow()
    {
        if(player == null)
        {
            stateMachine.ChangeState(EnemyStates.IDLE);
            return;
        }

        var direction = player.Transform.origin - this.Transform.origin;
        direction.y = 0;
        direction = direction.Normalized();

        translate = direction * moveSpeed;
    }


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
}
