public class WheatBlock : CubeBlock
{
    public override bool Breakable { get { return true; } }
    public override string[] TextureNames { get {
            return new[] {
                "blockWheat"
            };
        }
    }

    public override int GetTextureIndex(BlockFace face)
    {
        return 0;
    }
}
