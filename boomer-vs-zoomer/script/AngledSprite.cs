using System.Diagnostics;
using Godot;
using System;

public class AngledSprite : AnimatedSprite3D
{
    Spatial pivot;
    public override void _Ready()
    {
        pivot = GetParentSpatial();
    }
    public override void _Process(float delta)
    {
        if(pivot == null)
            return;

        if(pivot.RotationDegrees.y > 10)
            this.RotationDegrees = new Vector3(45, 0, 0);
        else
            this.RotationDegrees = new Vector3(-45, 0, 0);
    }
}
