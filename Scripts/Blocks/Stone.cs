public class Stone : CubeBlock
{
    public override bool Breakable { get { return true; } }
    public override string[] TextureNames { get { return new[] { "stone" }; } }

    public override int GetTextureIndex(BlockFace face)
    {
        return 0;
    }
}