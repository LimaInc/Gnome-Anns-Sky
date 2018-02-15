using System;
using Godot;

public class HabitationBlock : CubeBlock
{
    public override string[] TexturePaths { get { return new[] { "res://Images/habitationBlock.png" }; } }

    public override int GetTextureIndex(BlockFace face)
    {
        return 0;
    }
}