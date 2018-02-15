using System;
using Godot;

public class WorldGenerator
{
    OctaveNoise noise = new OctaveNoise(16);

    private byte stoneId = Game.GetBlockId<Stone>();
    private byte grassId = Game.GetBlockId<RedRock>();
    private byte habId = Game.GetBlockId<HabitationBlock>();

    private static float SIGMOID_PARAM_A = -40.0f;
    private static float SIGMOID_PARAM_B = 4.9f;

    private static float STARTING_HEIGHT = 55.0f;

    private static float BASE_RADIUS = 8.0f;
    private static float BASE_RADIUS_SQRD = BASE_RADIUS * BASE_RADIUS;

    public byte[,,] GetChunk(int x, int z, int sX, int sY, int sZ)
    {
        byte[,,] chunk = new byte[sX, sY, sZ];

        for(int i = 0; i < sX; i++)
        {
            for(int k = 0; k < sZ; k++)
            {
                int wx = x * sX + i;
                int wz = z * sZ + k;

                float xs = wx / 256.0f;
                float zs = wz / 256.0f;
                float height = noise.sample(xs, zs) * 128.0f + 50.0f;

                float centreDist = (float) Math.Sqrt(xs * xs + zs * zs);

                float sigmoidSample = sigmoid(centreDist, SIGMOID_PARAM_A, SIGMOID_PARAM_B);

                float weighted = sigmoidSample * STARTING_HEIGHT + (1 - sigmoidSample) * height;

                for(int j = 0; j < sY; j++)
                {
                    if(j < weighted)
                        chunk[i,j,k] = stoneId;
                    else if(j < weighted + 3) //3 layers of grass on top of stone
                        chunk[i,j,k] = grassId;

                    float startAdjustedY = j - STARTING_HEIGHT;
                    if (Math.Abs(wx) < BASE_RADIUS || Math.Abs(startAdjustedY) < BASE_RADIUS || Math.Abs(wz) < BASE_RADIUS)
                    {
                        float blockSphereDist = (float) Math.Sqrt(wx * wx + startAdjustedY * startAdjustedY + wz * wz);
                        if (Math.Abs(blockSphereDist - BASE_RADIUS) < 0.5f)
                        {
                            if (chunk[i,j,k] == 0)
                                chunk[i,j,k] = habId;
                        }
                        if (wx * wx + wz * wz < BASE_RADIUS_SQRD)
                        {
                            if (Math.Abs(j - (weighted + 2)) < 0.5f)
                            {
                                chunk[i,j,k] = habId;
                            }
                        }
                    }
                }
            }
        }

        return chunk;
    }

    public float sigmoid(float x, float a, float b)
    {
        return (float) Math.Exp(a * x + b) / (1.0f + (float) Math.Exp(a * x + b));
    }
}
