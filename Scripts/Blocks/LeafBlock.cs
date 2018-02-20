public class LeafBlock : CubeBlock
{
    public override string[] TexturePaths { get { return new[] { "res://Images/blockLeaf.png" }; } }

    public override int GetTextureIndex(BlockFace face)
    {
        return 0;
    }
}
