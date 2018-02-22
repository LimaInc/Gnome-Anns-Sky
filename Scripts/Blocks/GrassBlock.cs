public class GrassBlock : CubeBlock
{
    public override string[] TexturePaths { get { return new[] { "res://Images/grassTop.png", "res://Images/grassSide.png", "res://Images/brown_rock_side.png" }; } }

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
