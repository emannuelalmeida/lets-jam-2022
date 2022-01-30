using System.Collections.Generic;
using Godot;
using System;

public class ActionEvent
{
    public InputAction action;
    public bool released;

    public ActionEvent(InputAction action, bool released)
    {
        this.action = action;
        this.released = released;
    }
}
