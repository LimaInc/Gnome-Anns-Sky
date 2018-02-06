using System;
using System.Collections.Generic;
using Godot;

public enum BlockFace
{
    Top,
    Bottom,
    Left,
    Right,
    Front,
    Back
}
public abstract class Block
{
    //public Rect2 UVs { get; set; }
    abstract public string[] TexturePaths { get; }
    abstract public int GetTextureIndex(BlockFace face);
}
public class Stone : Block
{
    public override string[] TexturePaths { get { return new[] { "res://Images/stone.png" }; } }

    public override int GetTextureIndex(BlockFace face)
    {
        return 0;
    }
}
public class Grass : Block
{
    public override string[] TexturePaths { get { return new[] { "res://Images/brown_rock_top.png", "res://Images/brown_rock_side.png" }; } }

    public override int GetTextureIndex(BlockFace face)
    {
        switch(face)
        {
            case BlockFace.Top:
                return 0;
            default:
                return 1;
        }
    }
}