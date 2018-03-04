public class GrassBlock : CubeBlock
{
    public override bool Breakable { get { return true; } }
    public override string[] TextureNames { get {
            return new[] {
                "grassTop",
                "grassSide",
                "brown_rock_side"
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
