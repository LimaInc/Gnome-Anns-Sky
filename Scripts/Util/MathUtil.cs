using System;
using Godot;

class MathUtil
{
    public static float Sigmoid(float x, float a, float b)
    {
        return Mathf.Exp(a * x + b) / (1 + Mathf.Exp(a * x + b));
    }
}
