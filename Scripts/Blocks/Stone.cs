public class Stone : CubeBlock
{
    public override string[] TexturePaths { get { return new[] { Game.BLOCK_TEXTURE_PATH + "stone.png" }; } }

    public override int GetTextureIndex(BlockFace face)
    {
        return 0;
    }
}