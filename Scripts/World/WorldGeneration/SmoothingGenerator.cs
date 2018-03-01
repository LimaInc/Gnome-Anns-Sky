using Godot;

public class SmoothingGenerator : IGenerator
{
    private readonly byte STONE_ID = Game.GetBlockId<Stone>();
    private readonly byte RED_ROCK_ID = Game.GetBlockId<RedRock>();
    private const byte AIR_ID = WorldGenerator.AIR_ID;

    private readonly Vector2 center;
    private readonly int centerHeight;

    private readonly float smoothingAtCenter;
    private readonly float halfSmoothingDistance;

    private readonly float sigmoidParamB;
    private readonly float sigmoidParamA;

    public SmoothingGenerator(Vector2 center, int centerHeight, float smoothingAtCenter, float halfSmoothingDistance)
    {
        this.center = center;
        this.centerHeight = centerHeight;

        this.smoothingAtCenter = smoothingAtCenter;
        this.halfSmoothingDistance = halfSmoothingDistance;

        this.sigmoidParamB = Mathf.Log(this.smoothingAtCenter / (1 - this.smoothingAtCenter));
        this.sigmoidParamA = -this.sigmoidParamB / this.halfSmoothingDistance;
    }

    public void GenerateChunk(byte[,,] chunk, int[,] chunkTerrainHeight, IntVector2 chunkIndex, IntVector3 chunkSize)
    {
        // this is inaccurate because it tests just one corner of the chunk
        // and does not consider the pathological case of very sharp smoothing in the middle
        // TODO: fix
        float chunkCenterDist = (chunkIndex * new Vector2(chunkSize.x, chunkSize.z) - center).Length();
        if (MathUtil.Sigmoid(chunkCenterDist, sigmoidParamA, sigmoidParamB) * WorldGenerator.MAX_HEIGHT_DIFFERENCE < 0.5)
        {
            return;
        }

        for (int i = 0; i < chunkSize.x; i++)
        {
            for (int k = 0; k < chunkSize.z; k++)
            {
                IntVector2 worldCoords = new IntVector2(chunkIndex.x * chunkSize.x + i, chunkIndex.y * chunkSize.z + k);
                float height = chunkTerrainHeight[i, k];

                float centerDist = ((Vector2)worldCoords - center).Length();
                float sigmoidSample = MathUtil.Sigmoid(centerDist, sigmoidParamA, sigmoidParamB);
                chunkTerrainHeight[i, k] = (int)(sigmoidSample * centerHeight + (1 - sigmoidSample) * height);
            }
        }
    }
}
