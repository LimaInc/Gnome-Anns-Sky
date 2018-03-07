public class IceGenerator : IGenerator
{
    private readonly byte ICE_ID = Game.GetBlockId<IceBlock>();

    public readonly int seaLevel;

    public IceGenerator(int seaLevel)
    {
        this.seaLevel = seaLevel;
    } 

    public void GenerateChunk(byte[,,] chunk, int[,] chunkTerrainHeight, IntVector2 chunkIndex, IntVector3 chunkSize)
    {
        for (int i = 0; i < chunkSize.x; i++)
        {
            for (int k = 0; k < chunkSize.z; k++)
            {
                for (int j = chunkTerrainHeight[i, k] + 1; j < seaLevel; j++)
                {
                    chunk[i, j, k] = ICE_ID;
                }
            }
        }
    }
}
