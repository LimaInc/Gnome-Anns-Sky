public class Stone : CubeBlock
{
    public override string[] TexturePaths { get { return new[] { "res://Images/stone.png" }; } }

    public override int GetTextureIndex(BlockFace face)
    {
        return 0;
    }
}