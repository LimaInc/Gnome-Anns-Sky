using Godot;

public class IceBlock : CubeBlock
{
    public override bool Breakable { get { return true; } }
    public override string[] TextureNames { get { return new[] { "ice" }; } }

    public override int GetTextureIndex(BlockFace face)
    {
        return 0;
    }
}