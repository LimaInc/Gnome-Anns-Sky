public class LeafBlock : CubeBlock
{
    public override bool Breakable { get { return false; } }
    public override string[] TextureNames { get { return new[] { "blockLeaf" }; } }

    public override int GetTextureIndex(BlockFace face)
    {
        return 0;
    }
}
