public class RedRock : CubeBlock
{
    public override string[] TexturePaths { get { return new[] { "res://Images/brown_rock_top.png", "res://Images/brown_rock_side.png" }; } }

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