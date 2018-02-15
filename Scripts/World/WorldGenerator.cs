using System;
using Godot;

public class WorldGenerator
{
    OctaveNoise noise = new OctaveNoise(16);

    private byte stoneId = Game.GetBlockId<Stone>();
    private byte grassId = Game.GetBlockId<RedRock>();

    private static float SIGMOID_PARAM_A = -40.0f;
    private static float SIGMOID_PARAM_B = 4.9f;

    private static float STARTING_HEIGHT = 55.0f;

    public byte[,,] GetChunk(int x, int z, int sX, int sY, int sZ)
    {
        byte[,,] chunk = new byte[sX, sY, sZ];

        for(int i = 0; i < sX; i++)
        {
            for(int k = 0; k < sZ; k++)
            {
                float xs = (x * sX + i) / 256.0f;
                float zs = (z * sZ + k) / 256.0f;
                float height = noise.sample(xs, zs) * 128.0f + 50.0f;

                float centreDist = (float) Math.Sqrt(xs * xs + zs * zs);

                float sigmoidSample = sigmoid(centreDist, SIGMOID_PARAM_A, SIGMOID_PARAM_B);

                float weighted = sigmoidSample * STARTING_HEIGHT + (1 - sigmoidSample) * height;

                for(int j = 0; j < sY; j++)
                {
                    if(j < weighted)
                        chunk[i,j,k] = stoneId;
                    else if(j < weighted + 3) //3 layers of rock on top of stone
                        chunk[i,j,k] = grassId;
                    else
                        chunk[i,j,k] = 0;
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
