using Godot;
using System;

public class Noise
{
    public static void noise()
    {
        
    }

    public static int intToNat(int z)
    {
        if (z < 0)
            return -2 * z - 1;
        return 2 * z;
    }

    public static int natToInt(int n)
    {
        if (n % 2 == 0)
            return n / 2;
        return -(n + 1) / 2;
    }

    public static int cantorPairNats(int x, int y)
    {
        return (int) (0.5 * (x + y) * (x + y + 1) + y);
    }

    public static int[] deCantorPair(int z)
    {
        int t = (int) (Math.Floor((Math.Sqrt(8 * z + 1) - 1) / 2));
        int x = t * (t + 3) / 2 - z;
        int y = z - t * (t + 1) / 2;
        return new int[] {x,y};
    }
}