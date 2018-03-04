public class RedRock : CubeBlock
{
    public override bool Breakable { get { return true; } }
    public override string[] TextureNames { get {
            return new[] {
                "brown_rock_top",
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
            default:
                return 1;
        }
    }
}