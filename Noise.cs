using Godot;
using System;

public class Noise
{
    public static void noise()
    {
        // for (int i = -100; i < 100; i++)
        //     GD.Print(i + " ," + natToInt(intToNat(i)));

        // for (int i = 0; i < 200; i++)
        //     GD.Print(i + " ," + intToNat(natToInt(i)));
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

    private static int floorMod(int a, int b)
    {
        return (Math.Abs(a * b) + a) % b;
    }
}