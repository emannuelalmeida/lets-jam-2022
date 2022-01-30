using Godot;
using System;
using System.Linq;

public class CameraMovement : Camera
{
    [Export]
    float minX;

    [Export]
    float maxX;

    Area enemyDetectionArea;
    Spatial cameraPivot;
    KinematicBody player;

    bool canMove;
    float step = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        player = GetTree().CurrentScene.GetNode<KinematicBody>("Player");
        enemyDetectionArea = GetNode<Area>("EnemyDetectionArea");
        cameraPivot = GetParentSpatial();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        var enemyCount1 = enemyDetectionArea.GetOverlappingBodies().Count;
        var enemyCount = enemyDetectionArea.GetOverlappingBodies().Cast<Node>().Where(x => x.IsInGroup("Enemy")).Count();
        canMove = enemyCount == 0;
        
        GD.Print($"Enemies {enemyCount1} {enemyCount}");

        if(canMove)
        {
            if(step < 1)
                step += 1 * delta;
            if(step > 1)
                step = 1;

            float newX = cameraPivot.Translation.x;
            newX = player.Translation.x;
            newX = Math.Max(minX, newX);
            newX = Math.Min(maxX, newX);

            var targetPos = new Vector3(newX, cameraPivot.Translation.y, cameraPivot.Translation.z);
            this.cameraPivot.Translation = this.cameraPivot.Translation.LinearInterpolate(targetPos, step);
        }
        else
            step = 0;
    }
}
