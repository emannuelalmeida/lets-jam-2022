using System.Diagnostics;
using Godot;
using System;

public class AngledSprite : AnimatedSprite3D
{
    public override void _Ready()
    {
        this.RotationDegrees = new Vector3(-45, 0, 0);
    }
    public override void _Process(float delta)
    {
     
    }
}
