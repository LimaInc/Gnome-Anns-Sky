public class TreeBlock : CubeBlock
{
    public override string[] TexturePaths { get { return new[] { "res://Images/blockTree.png" }; } }

    public override int GetTextureIndex(BlockFace face)
    {
        return 0;
    }
}
