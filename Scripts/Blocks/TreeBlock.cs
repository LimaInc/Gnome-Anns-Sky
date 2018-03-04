public class TreeBlock : CubeBlock
{
    public override bool Breakable { get { return false; } }
    public override string[] TextureNames { get { return new[] { "blockTree" }; } }

    public override int GetTextureIndex(BlockFace face)
    {
        return 0;
    }
}
