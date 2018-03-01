public class RockGenerator : IGenerator
{
    private readonly byte STONE_ID = Game.GetBlockId<Stone>();
    private readonly byte RED_ROCK_ID = Game.GetBlockId<RedRock>();

    public const int RED_ROCK_HEIGHT = 3;

    public void GenerateChunk(byte[,,] chunk, int[,] chunkTerrainHeight, IntVector2 chunkIndex, IntVector3 chunkSize)
    {
        for (int i = 0; i < chunkSize.x; i++)
        {
            for (int k = 0; k < chunkSize.z; k++)
            {
                for (int j = 0; j < chunkSize.y; j++)
                {
                    if (j <= chunkTerrainHeight[i, k] - RED_ROCK_HEIGHT)
                        chunk[i, j, k] = STONE_ID;
                    else if (j <= chunkTerrainHeight[i, k])
                        chunk[i, j, k] = RED_ROCK_ID;
                }
            }
        }
    }
}
