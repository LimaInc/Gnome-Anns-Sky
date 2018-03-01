public class RockGenerator : IGenerator
{
    private readonly byte STONE_ID = Game.GetBlockId<Stone>();
    private readonly byte RED_ROCK_ID = Game.GetBlockId<RedRock>();

    public readonly int redRockDepth;

    public RockGenerator(int redRockHeight)
    {
        this.redRockDepth = redRockHeight;
    } 

    public void GenerateChunk(byte[,,] chunk, int[,] chunkTerrainHeight, IntVector2 chunkIndex, IntVector3 chunkSize)
    {
        for (int i = 0; i < chunkSize.x; i++)
        {
            for (int k = 0; k < chunkSize.z; k++)
            {
                for (int j = 0; j <= chunkTerrainHeight[i, k]; j++)
                {
                    if (j <= chunkTerrainHeight[i, k] - redRockDepth)
                    {
                        chunk[i, j, k] = STONE_ID;
                    }
                    else
                    {
                        chunk[i, j, k] = RED_ROCK_ID;
                    }
                }
            }
        }
    }
}
