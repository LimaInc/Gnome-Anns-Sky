using System;
using Godot;

public class WorldGenerator
{
    OctaveNoise noise = new OctaveNoise(16);

    private byte stoneId = Game.GetBlockId<Stone>();
    private byte grassId = Game.GetBlockId<RedRock>();

    public byte[,,] GetChunk(int x, int z, int sX, int sY, int sZ)
    {
        byte[,,] chunk = new byte[sX, sY, sZ];

        for(int i = 0; i < sX; i++)
        {
            for(int k = 0; k < sZ; k++)
            {
                float xs = (x * sX + i) / 256.0f;
                float zs = (z * sZ + k) / 256.0f;
                int height = (int) (noise.sample(xs, zs) * 128) + 50;

                for(int j = 0; j < sY; j++)
                {
                    if(j < height)
                        chunk[i,j,k] = stoneId;
                    else if(j < height + 3) //3 layers of rock on top of stone
                        chunk[i,j,k] = grassId;
                    else
                        chunk[i,j,k] = 0;
                }
            }
        }

        return chunk;
    }
}
