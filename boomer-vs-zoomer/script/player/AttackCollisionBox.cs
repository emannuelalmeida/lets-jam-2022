using System.Collections.Generic;
using Godot;
using System;

public class AttackCollisionBox
{
    public int x;
    public int y;
    public int z;
    public int width;
    public int height;
    public int depth;

    public AttackCollisionBox(int x, int z, int y, int width, int depth, int height)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.width = width;
        this.depth = depth;
        this.height = height;
    }
}
