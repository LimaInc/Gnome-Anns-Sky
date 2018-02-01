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

    OctaveNoise noise = new OctaveNoise(16);

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
                        chunk[i,j,k] = 1;
                    else if(j < height + 3) //3 layers of grass on top of rock
                        chunk[i,j,k] = 2;
                    else
                        chunk[i,j,k] = 0;
                }
            }
        }

        return chunk;
    }
}
