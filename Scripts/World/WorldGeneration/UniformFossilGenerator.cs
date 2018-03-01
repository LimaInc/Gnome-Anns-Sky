using System;

public class UniformFossilGenerator : IGenerator
{
    private readonly byte fossilID;
    private readonly int fossilDepth;
    private readonly float spawnChance;
    private readonly int i;

    private Random random;

    public UniformFossilGenerator(byte fossilID, int fossilDepth, float spawnChance)
    {
        this.fossilID = fossilID;
        this.fossilDepth = fossilDepth;
        this.spawnChance = spawnChance;
        
        this.i = MathUtil.GlobalRandom.Next();
    } 

    public void GenerateChunk(byte[,,] chunk, int[,] chunkTerrainHeight, IntVector2 chunkIndex, IntVector3 chunkSize)
    {
        this.random = new Random(new IntVector3(this.i, chunkIndex.x, chunkIndex.y).GetHashCode());

        for (int i = 0; i < chunkSize.x; i++)
        {
            for (int k = 0; k < chunkSize.z; k++)
            {
                for (int j = 0; j < fossilDepth; j++)
                {
                    if (random.NextDouble() < spawnChance)
                    {
                        chunk[i, chunkTerrainHeight[i,k] - j, k] = fossilID;
                    }
                }
            }
        }
    }
}
