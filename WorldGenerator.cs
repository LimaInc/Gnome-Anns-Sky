using System;
using Godot;

public class WorldGenerator
{
    //Rect at index i tells you the UVs for block i+1 (because air=0 and has no texture)
    //Hardcoded for now, but we might want a better solution. Perhaps automatic texture packer?
    //Probably want to support multiple textures for a single block at different rotations, e.g. grass
    public Rect2[] blockUVs = {
        new Rect2(0.0f, 0.0f, 0.5f, 1.0f),
        new Rect2(0.5f, 0.0f, 0.5f, 1.0f)
    };

    Noise noise = new Noise();
    private float GetNoise(int x, int z)
    {
        float fx = x * 1.1f; float fz = z * 1.1f; //Because perlin noise is the same value at integer samples

        return noise.sample(fx / 16, fz / 16) * 8 + noise.sample(fx / 4, fz / 4) * 1 + 16;
    }

    public byte[,,] GetChunk(int x, int y, int z, int sX, int sY, int sZ)
    {
        byte[,,] chunk = new byte[sX, sY, sZ];

        for(int i = 0; i < sX; i++)
        {
            for(int k = 0; k < sZ; k++)
            {
                int height = (int)GetNoise(x + i, z + k);

                for(int j = 0; j < sY; j++)
                {
                    if((j + y) < height)
                        chunk[i,j,k] = 1;
                    else if((j + y) < height + 3) //3 layers of grass on top of rock
                        chunk[i,j,k] = 2;
                    else
                        chunk[i,j,k] = 0;
                }
            }
        }

        return chunk;
    }
}
