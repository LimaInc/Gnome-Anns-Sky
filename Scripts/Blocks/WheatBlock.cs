public class WheatBlock : CubeBlock
{
    public override bool Breakable { get { return true; } }
    public override string[] TexturePaths { get {
            return new[] {
                Game.BLOCK_TEXTURE_PATH + "blockWheat.png",
            };
        }
    }

    public override int GetTextureIndex(BlockFace face)
    {
        return 0;
    }
}
