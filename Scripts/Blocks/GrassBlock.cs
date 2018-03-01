public class GrassBlock : CubeBlock
{
    public override bool Breakable { get { return true; } }
    public override string[] TexturePaths { get {
            return new[] {
                Game.BLOCK_TEXTURE_PATH + "grassTop.png",
                Game.BLOCK_TEXTURE_PATH + "grassSide.png",
                Game.BLOCK_TEXTURE_PATH + "brown_rock_side.png"
            };
        }
    }

    public override int GetTextureIndex(BlockFace face)
    {
        switch(face)
        {
            case BlockFace.Top:
                return 0;
            case BlockFace.Bottom:
                return 2;
            default:
                return 1;
        }
    }
}
