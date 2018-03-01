using System;
using Godot;

public class WorldGenerator
{
    OctaveNoise noise = new OctaveNoise(16);

    private readonly byte stoneId = Game.GetBlockId<Stone>();
    private readonly byte grassId = Game.GetBlockId<RedRock>();
    private readonly byte iceId = Game.GetBlockId<IceBlock>();
    private readonly byte habId = Game.GetBlockId<HabitationBlock>();
    private readonly byte defossiliserId = Game.GetBlockId<DefossiliserBlock>();
    private readonly byte airId = 0;

    private static float SIGMOID_PARAM_A = -40.0f;
    private static float SIGMOID_PARAM_B = 4.9f;

    private static float STARTING_HEIGHT = 55.0f;

    private static int SEA_LEVEL = 30;

    public static float BASE_RADIUS = 8.0f;
    public static float BASE_RADIUS_SQRD = BASE_RADIUS * BASE_RADIUS;

    public static int GRASS_HEIGHT = 3;

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
                float height = noise.sample(xs, zs) * 128.0f + STARTING_HEIGHT;

                float centreDist = (float) Math.Sqrt(xs * xs + zs * zs);

                float sigmoidSample = Sigmoid(centreDist, SIGMOID_PARAM_A, SIGMOID_PARAM_B);

                float weighted = sigmoidSample * STARTING_HEIGHT + (1 - sigmoidSample) * height;

                for(int j = 0; j < sY; j++)
                {
                    if(j < SEA_LEVEL)
                        chunk[i,j,k] = iceId;
                    else if(j < weighted)
                        chunk[i,j,k] = stoneId;
                    else if(j < weighted + GRASS_HEIGHT) //3 layers of grass on top of stone
                        chunk[i,j,k] = grassId;

                    float startAdjustedY = j - STARTING_HEIGHT;
                    if ((Math.Abs(wx) < BASE_RADIUS || 
                        Math.Abs(startAdjustedY) < BASE_RADIUS || 
                        Math.Abs(wz) < BASE_RADIUS) && 
                        wx < BASE_RADIUS - GRASS_HEIGHT)
                    {
                        float blockSphereDist = (float) Math.Sqrt(wx * wx + startAdjustedY * startAdjustedY + wz * wz);
                        if (blockSphereDist < BASE_RADIUS && startAdjustedY > GRASS_HEIGHT - 1)
                        {
                            chunk[i, j, k] = airId;
                        }
                        if (Math.Abs(blockSphereDist - BASE_RADIUS) < 0.5f && startAdjustedY > GRASS_HEIGHT - 1)
                        {
                            chunk[i,j,k] = habId;
                        }
                        if (wx * wx + wz * wz < BASE_RADIUS_SQRD)
                        {
                            if (Math.Abs(j - (weighted + GRASS_HEIGHT - 1)) < 0.5f)
                            {
                                chunk[i,j,k] = habId;
                            }
                        }
                    }
                }
            }
        }
        if(x==0 && z==-1)
            chunk[0, (int)STARTING_HEIGHT + GRASS_HEIGHT, sZ - (int)BASE_RADIUS + 2] = defossiliserId;

        return chunk;
    }

    public float Sigmoid(float x, float a, float b)
    {
        return (float) Math.Exp(a * x + b) / (1.0f + (float) Math.Exp(a * x + b));
    }
}
