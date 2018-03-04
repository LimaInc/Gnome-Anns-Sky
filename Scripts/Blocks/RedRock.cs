public class RedRock : CubeBlock
{
    public override bool Breakable { get { return true; } }
    public override string[] TexturePaths { get {
            return new[] {
                Game.BLOCK_TEXTURES_DIR_PATH + "brown_rock_top.png",
                Game.BLOCK_TEXTURES_DIR_PATH + "brown_rock_side.png"
            };
        }
    }

    public override int GetTextureIndex(BlockFace face)
    {
        switch(face)
        {
            case BlockFace.Top:
                return 0;
            default:
                return 1;
        }
    }
}