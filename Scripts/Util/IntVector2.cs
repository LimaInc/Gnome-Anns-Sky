using System;
using Godot;

public struct IntVector2 : IEquatable<IntVector2> //Used for chunk/block positions
{
    public int x;
    public int y;

    public IntVector2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static IntVector2 operator +(IntVector2 left, IntVector2 right)
    {
        left.x += right.x;
        left.y += right.y;
        return left;
    }

    public static IntVector2 operator -(IntVector2 left, IntVector2 right)
    {
        left.x -= right.x;
        left.y -= right.y;
        return left;
    }

    public static IntVector2 operator -(IntVector2 vec)
    {
        vec.x = -vec.x;
        vec.y = -vec.y;
        return vec;
    }

    public static IntVector2 operator *(IntVector2 left, IntVector2 right)
    {
        left.x *= right.x;
        left.y *= right.y;
        return left;
    }

    public static bool operator ==(IntVector2 left, IntVector2 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(IntVector2 left, IntVector2 right)
    {
        return !left.Equals(right);
    }

    public override bool Equals(object obj)
    {
        if(obj is IntVector2)
        {
            return Equals((IntVector2)obj);
        }

        return false;
    }

    public bool Equals(IntVector2 other)
    {
        return x == other.x && y == other.y;
    }

    public override int GetHashCode()
    {
        return y.GetHashCode() ^ x.GetHashCode();
    }

    public override string ToString()
    {
        return String.Format("({0}, {1})", this.x.ToString(), this.y.ToString());
    }

    public string ToString(string format)
    {
        return String.Format("({0}, {1})", this.x.ToString(format), this.y.ToString(format));
    }
}