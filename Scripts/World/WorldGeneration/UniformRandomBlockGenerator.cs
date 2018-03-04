using System;

public class UniformRandomBlockGenerator : IGenerator
{
    private readonly byte blockID;
    private readonly int depthMin;
    private readonly int depthMax;
    private readonly float spawnChance;
    private readonly int i;

    private Random random;

    public UniformRandomBlockGenerator(byte blockID_, int depthMin_, int depthMax_, float spawnChance_)
    {
        blockID = blockID_;
        depthMin = depthMin_;
        depthMax = depthMax_;
        spawnChance = spawnChance_;

        i = MathUtil.GlobalRandom.Next();
    } 

    public void GenerateChunk(byte[,,] chunk, int[,] chunkTerrainHeight, IntVector2 chunkIndex, IntVector3 chunkSize)
    {
        this.random = new Random(new IntVector3(this.i, chunkIndex.x, chunkIndex.y).GetHashCode());

        for (int i = 0; i < chunkSize.x; i++)
        {
            for (int k = 0; k < chunkSize.z; k++)
            {
                for (int j = depthMin; j < depthMax; j++)
                {
                    if (random.NextDouble() < spawnChance)
                    {
                        chunk[i, chunkTerrainHeight[i,k] - j, k] = blockID;
                    }
                }
            }
        }
    }
}
