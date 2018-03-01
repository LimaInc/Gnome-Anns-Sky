using System;
using Godot;

public class WorldGenerator
{
    OctaveNoise noise = new OctaveNoise(16);

    private readonly byte STONE_ID = Game.GetBlockId<Stone>();
    private readonly byte RED_ROCK_ID = Game.GetBlockId<RedRock>();
    private readonly byte HAB_ID = Game.GetBlockId<HabitationBlock>();
    private readonly byte DEFOSSILISER_ID = Game.GetBlockId<DefossiliserBlock>();
    private const byte AIR_ID = 0;

    private const float SMOOTHING_AT_BASE_CENTER = 0.999f;
    private const float SMOOTHING_AT_BASE_RADIUS = 0.99f;

    private static readonly float SIGMOID_PARAM_B = 
        Mathf.Log(SMOOTHING_AT_BASE_CENTER / (1 - SMOOTHING_AT_BASE_CENTER));
    private static readonly float SIGMOID_PARAM_A = 
        (Mathf.Log(SMOOTHING_AT_BASE_RADIUS / (1 - SMOOTHING_AT_BASE_RADIUS)) - SIGMOID_PARAM_B) / BASE_RADIUS;

    public const int RED_ROCK_HEIGHT = 3;

    public const float BASE_RADIUS = 8;
    public const int BASE_ENTRANCE_DEPTH = 2;
    public const int BASE_FLOOR_HEIGHT = RED_ROCK_HEIGHT - 1;
    public const float BASE_RADIUS_SQRD = BASE_RADIUS * BASE_RADIUS;

    public static readonly IntVector2 BASE_XZ_POSITION = new IntVector2(-(int)BASE_RADIUS, 0);
    private const int BASE_Y_POSITION = 55;

    // with respect to the base
    public static readonly IntVector3 DEFOSSILISER_LOCAL_POSITION = new IntVector3(0, BASE_FLOOR_HEIGHT + 1, 2 - (int) BASE_RADIUS);

    public byte[,,] GetChunk(IntVector2 chunkIndex, IntVector3 chunkSize)
    {
        byte[,,] chunk = new byte[chunkSize.x, chunkSize.y, chunkSize.z];

        // TODO: might be useful to somehow persistently associate this with chunk
        int[,] chunkTerrainHeight = GetChunkTerrainHeight(chunkIndex, chunkSize);

        GenerateRocks(chunk, chunkTerrainHeight, chunkIndex, chunkSize);
        GenerateBase(chunk, chunkTerrainHeight, chunkIndex, chunkSize);

        return chunk;
    }

    private int[,] GetChunkTerrainHeight(IntVector2 chunkIndex, IntVector3 chunkSize)
    {
        int[,] chunkHeight = new int[chunkSize.x, chunkSize.z];

        for (int i = 0; i < chunkSize.x; i++)
        {
            for (int k = 0; k < chunkSize.z; k++)
            {
                IntVector2 worldCoords = new IntVector2(chunkIndex.x * chunkSize.x + i, chunkIndex.y * chunkSize.z + k);

                Vector2 fractionalCoords = (Vector2)worldCoords / 256;

                float height = noise.Sample(fractionalCoords.x, fractionalCoords.y) * 128 + BASE_Y_POSITION;

                // by letting sigmoidSample ~= 0 for small centreDist and 1 otherwise
                // creates a plateau where the base is
                float baseDist = (worldCoords - BASE_XZ_POSITION).Length();
                float sigmoidSample = MathUtil.Sigmoid(baseDist, SIGMOID_PARAM_A, SIGMOID_PARAM_B);
                float weighted = sigmoidSample * BASE_Y_POSITION + (1 - sigmoidSample) * height;

                chunkHeight[i, k] = (int)weighted;
            }
        }

        return chunkHeight;
    }

    private void GenerateRocks(byte[,,] chunk, int[,] chunkTerrainHeight, IntVector2 chunkIndex, IntVector3 chunkSize)
    {
        for (int i = 0; i < chunkSize.x; i++)
        {
            for (int k = 0; k < chunkSize.z; k++)
            {
                for (int j = 0; j < chunkSize.y; j++)
                {
                    if (j <= chunkTerrainHeight[i,k])
                        chunk[i, j, k] = STONE_ID;
                    else if (j <= chunkTerrainHeight[i, k] + RED_ROCK_HEIGHT)
                        chunk[i, j, k] = RED_ROCK_ID;
                }
            }
        }
    }

    private void GenerateBase(byte[,,] chunk, int[,] chunkTerrainHeight, IntVector2 chunkIndex, IntVector3 chunkSize)
    {
        for (int i = 0; i < chunkSize.x; i++)
        {
            for (int k = 0; k < chunkSize.z; k++)
            {
                IntVector2 worldCoords = new IntVector2(chunkIndex.x * chunkSize.x + i, chunkIndex.y * chunkSize.z + k);
                IntVector2 localCoords = worldCoords - BASE_XZ_POSITION;

                for (int baseY = BASE_FLOOR_HEIGHT; baseY <= BASE_RADIUS; baseY++)
                {
                    int j = baseY + BASE_Y_POSITION;

                    if (localCoords.LengthSquared() < BASE_RADIUS_SQRD && localCoords.x < BASE_RADIUS - BASE_ENTRANCE_DEPTH)
                    {
                        IntVector3 local3DCoords = new IntVector3(localCoords.x, baseY, localCoords.y);
                        float blockSphereDist = local3DCoords.Length();
                        // prepare space for base
                        if (blockSphereDist < BASE_RADIUS)
                        {
                            chunk[i, j, k] = AIR_ID;
                        }
                        // generate base walls (floor and dome) 
                        if (baseY == BASE_FLOOR_HEIGHT || Math.Abs(blockSphereDist - BASE_RADIUS) < 0.5)
                        {
                            chunk[i, j, k] = HAB_ID;
                        }
                        // generate defossiliser
                        if (local3DCoords == DEFOSSILISER_LOCAL_POSITION)
                        {
                            chunk[i, j, k] = DEFOSSILISER_ID;
                        }
                    }
                }
            }
        }
    }
}
