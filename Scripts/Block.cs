using System;
using System.Collections.Generic;
using Godot;

public abstract class Block
{
    public Rect2 UVs { get; set; }
    abstract public string TexturePath { get; }
}
public class Stone : Block
{
    public override string TexturePath { get { return "res://Images/stone.png"; } }
}
public class Grass : Block
{
    public override string TexturePath { get { return "res://Images/grass.png"; } }
}