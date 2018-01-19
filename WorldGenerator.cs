using System;

public class WorldGenerator
{
    Noise noise = new Noise();
    private float GetNoise(int x, int z)
    {
        float fx = x * 1.1f; float fz = z * 1.1f; //Because perlin noise is the same value at integer samples

        return noise.sample(fx / 16, fz / 16) * 8 + noise.sample(fx / 4, fz / 4) * 1 + 16;
    }

    public byte[,,] GetChunk(int x, int y, int z, int sX, int sY, int sZ)
    {
        byte[,,] chunk = new byte[sX, sY, sZ];

        for(int i = x; i < x + sX; i++)
        {
            for(int k = z; k < z + sZ; k++)
            {
                int height = (int)GetNoise(i,k);

                for(int j = y; j < y + sY; j++)
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
