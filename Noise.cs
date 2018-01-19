using Godot;
using System;
using System.Collections.Generic;

public class Noise
{
    private Random random = new Random();
    private Dictionary<int, Vector2> randGrads = new Dictionary<int, Vector2>();

    public static void noise()
    {
    }

    private static int intToNat(int z)
    {
        if (z < 0)
            return -2 * z - 1;
        return 2 * z;
    }

    private static int natToInt(int n)
    {
        if (n % 2 == 0)
            return n / 2;
        return -(n + 1) / 2;
    }

    private static int cantorPairNats(int x, int y)
    {
        return (int) (0.5 * (x + y) * (x + y + 1) + y);
    }

    private static int[] deCantorPair(int z)
    {
        int t = (int) (Math.Floor((Math.Sqrt(8 * z + 1) - 1) / 2));
        int x = t * (t + 3) / 2 - z;
        int y = z - t * (t + 1) / 2;
        return new int[] {x,y};
    }

    public Vector2 getGrad(int x, int y)
    {
        int nx = intToNat(x);
        int ny = intToNat(y);

        int z = cantorPairNats(nx,ny);

        if(randGrads.ContainsKey(z))
            return randGrads[z];

        float xx = (float) random.NextDouble() * 2 - 1;
        float yy = (float) random.NextDouble() * 2 - 1;
        Vector2 v = new Vector2(xx, yy);
        v = v.Normalized();
        randGrads.Add(z, v);
        return v;
    }
}