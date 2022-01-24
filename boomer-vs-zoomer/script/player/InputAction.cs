using System.Collections.Generic;
using Godot;
using System;

public class InputAction
{
    public string name;
    public EventType eventType;
    public List<ActionSequence> sequence;
    public int priority;
    public List<string> startStates;
    public string changeState;
    public string releaseState;

    public bool startStateMatches(Behavior state)
    {
        foreach (var startState in startStates)
        {
            if (startState == state.name)
            {
                if(name == "StartDashLeft" || name == "StartDashRight")
                    GD.Print("dash!");
                return true;
            }

            foreach (var flag in state.flags)
            {
                if (startState == flag)
                    return true;
            }
        }

        return false;
    }
}
