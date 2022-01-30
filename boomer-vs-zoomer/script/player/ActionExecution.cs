using System.Collections.Generic;
using Godot;
using System;

public class ActionExecution
{
    public InputAction action;
    public int currentState;
    public int ticks;

    public ActionExecution(InputAction action)
    {
        this.action = action;
        currentState = 0;
        ticks = 0;
    }

    public bool updateState(List<string> input, int inputTicks)
    {
        ActionSequence seq = action.sequence[currentState];
        if (inputTicks - ticks > seq.expirationTicks)
        {
            currentState = 0;
            ticks = 0;
        }

        if (seq.matches(input))
        {
            currentState++;
            ticks = inputTicks;

            if (currentState >= action.sequence.Count)
            {
                currentState = 0;
                ticks = 0;
                return true;
            }
        }

        return false;
    }
}
