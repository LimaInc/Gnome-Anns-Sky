public abstract class FossilBlock : CubeBlock
{
    private readonly string[] texturePaths; 

    public override string[] TexturePaths { get => texturePaths; }

    public FossilBlock(string texture)
    {
        texturePaths = new[] { Game.BLOCK_TEXTURE_PATH + texture };
    }

    public override int GetTextureIndex(BlockFace face) => 0;
}