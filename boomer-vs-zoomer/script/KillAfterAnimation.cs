using Godot;
using System;

public class KillAfterAnimation : AngledSprite
{
    public override void _Ready()
    {
        Connect("animation_finished", this, "OnFinishAnimation");
        Playing = true;
    }

    public void OnFinishAnimation()
    {
        Owner.QueueFree();
    }
}
