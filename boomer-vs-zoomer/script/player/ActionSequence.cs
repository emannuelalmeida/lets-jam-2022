using System.Collections.Generic;
using Godot;
using System;

public class ActionSequence
{
    public List<String> keys;
    public bool pressed;
    public uint expirationTicks;

    public ActionSequence(List<string> keys, bool pressed, uint expirationTicks = 5)
    {
        this.keys = keys;
        this.pressed = pressed;
        this.expirationTicks = expirationTicks;
    }

    public bool matches(List<string> input)
    {
        foreach (var key in keys)
        {
            if (input.Contains(key) != pressed)
            {
                return false;
            }
        }

        return true;
    }

    public class Press : ActionSequence
    {
        public Press(string key, uint expirationTicks = 15) : base(new List<string> { key }, true, expirationTicks)
        {
            
        }
        public Press(List<string> keys, uint expirationTicks = 15) : base(keys, true, expirationTicks)
        {
            
        }
    }

    public class Release : ActionSequence
    {
        public Release(string key, uint expirationTicks = 15) : base(new List<string> { key }, false, expirationTicks)
        {
            
        }
        public Release(List<string> keys, uint expirationTicks = 15) : base(keys, false, expirationTicks)
        {
            
        }
    }
}
