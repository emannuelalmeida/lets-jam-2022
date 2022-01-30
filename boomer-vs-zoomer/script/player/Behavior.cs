using System.Collections.Generic;
using Godot;
using System;


public class Behavior
{

    public string name;
    public int animation;
    public int paySoundId;
    public bool initial;
    public bool idle;
    public bool damage;
    public bool die;
    public List<string> flags;
    public Updater<Player> updater;

    public Behavior()
    {
    }

    public Behavior(string name)
    {
        this.name = name;
    }
}
