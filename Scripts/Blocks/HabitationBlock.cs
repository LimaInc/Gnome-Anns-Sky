using System;
using Godot;

public class HabitationBlock : CubeBlock
{
    public override bool Breakable { get { return false; } }
    public override string[] TextureNames { get { return new[] { "habitationBlock" }; } }

    public override int GetTextureIndex(BlockFace face)
    {
        return 0;
    }
}