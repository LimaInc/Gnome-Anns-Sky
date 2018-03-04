public abstract class FossilBlock : CubeBlock
{
    private readonly string[] texturePaths; 

    public override bool Breakable { get { return true; } }
    public override string[] TextureNames { get => texturePaths; }

    public FossilBlock(string texture)
    {
        texturePaths = new[] { texture };
    }

    public override int GetTextureIndex(BlockFace face) => 0;
}