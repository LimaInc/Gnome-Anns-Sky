using System;
using System.Collections.Generic;
using Godot;

public class WorldGenerator
{
    public const byte AIR_ID = 0;

    public readonly IList<ITerrainModifier> terrainModifiers;
    public readonly IList<IGenerator> generators;

    public WorldGenerator()
    {
        terrainModifiers = new List<ITerrainModifier>();
        generators = new List<IGenerator>();
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

    public int GetHeightAt(Vector2 worldCoords)
    {
        int h = 0;
        foreach (ITerrainModifier terrainModifier in terrainModifiers)
        {
            terrainModifier.UpdateHeight(worldCoords, ref h);
        }
        return h;
    }

    private int[,] GetChunkTerrainHeight(IntVector2 chunkIndex, IntVector3 chunkSize)
    {
        int[,] chunkHeight = new int[chunkSize.x, chunkSize.z];

        for (int i = 0; i < chunkSize.x; i++)
        {
            for (int k = 0; k < chunkSize.z; k++)
            {
                IntVector2 worldCoords = new IntVector2(chunkIndex.x * chunkSize.x + i, chunkIndex.y * chunkSize.z + k);
                chunkHeight[i, k] = GetHeightAt((Vector2)worldCoords);
            }
        }

        return chunkHeight;
    }
}
