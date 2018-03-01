using System;
using System.Collections.Generic;
using Godot;

public class WorldGenerator
{
    public const byte AIR_ID = 0;

    OctaveNoise noise = new OctaveNoise(16);

    public const int MAX_HEIGHT_DIFFERENCE = HEIGHT_SPREAD * 2;
    private const int AVERAGE_HEIGHT = 55;
    private const int HEIGHT_SPREAD = 128;

    private List<IGenerator> generators;

    public WorldGenerator(params IGenerator[] generators)
    {
        this.generators = new List<IGenerator>(generators);
    }

    public byte[,,] GetChunk(IntVector2 chunkIndex, IntVector3 chunkSize)
    {
        byte[,,] chunk = new byte[chunkSize.x, chunkSize.y, chunkSize.z];

        // TODO: might be useful to somehow persistently associate this with chunk
        int[,] chunkTerrainHeight = GetChunkTerrainHeight(chunkIndex, chunkSize);

        foreach(IGenerator gen in generators)
        {
            gen.GenerateChunk(chunk, chunkTerrainHeight, chunkIndex, chunkSize);
        }

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
                chunkHeight[i, k] = (int)(noise.Sample(fractionalCoords.x, fractionalCoords.y) * HEIGHT_SPREAD) + AVERAGE_HEIGHT;
            }
        }

        return chunkHeight;
    }
}
