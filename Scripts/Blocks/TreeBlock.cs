public class TreeBlock : CubeBlock
{
    public override string[] TexturePaths { get { return new[] { Game.BLOCK_TEXTURE_PATH + "blockTree.png" }; } }

    public override int GetTextureIndex(BlockFace face)
    {
        return 0;
    }
}
