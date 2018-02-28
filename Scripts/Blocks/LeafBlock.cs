public class LeafBlock : CubeBlock
{
    public override string[] TexturePaths { get { return new[] { Game.BLOCK_TEXTURE_PATH + "blockLeaf.png" }; } }

    public override int GetTextureIndex(BlockFace face)
    {
        return 0;
    }
}
