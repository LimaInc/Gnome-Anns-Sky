using System;
using Godot;

public static class MathUtil
{
    private static Random r = new Random();

    public static Random GlobalRandom { get => r; }

    public static float Sigmoid(float x, float a, float b)
    {
        return Mathf.Exp(a * x + b) / (1 + Mathf.Exp(a * x + b));
    }

    public static int RoundDownDiv(int a, int b)
    {
        if (a >= 0)
        {
            return a / b;
        }
        else
        {
            return (a - b + 1) / b;
        }
    }

    public static Vector2 Get(this Rect2 r, int i, int j)
    {
        return new Vector2(i == 0 ? r.Position.x : r.End.x, j == 0 ? r.Position.y : r.End.y);
    }
}
