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
        return new IntVector2(left.x + right.x, left.x + right.x);
    }

    public static IntVector2 operator -(IntVector2 left, IntVector2 right)
    {
        return new IntVector2(left.x - right.x, left.x - right.x);
    }

    public static IntVector2 operator -(IntVector2 vec)
    {
        return new IntVector2(- vec.x, -vec.y);
    }

    public static IntVector2 operator *(IntVector2 left, int factor)
    {
        return new IntVector2(left.x * factor, left.y * factor);
    }

    public static IntVector2 operator *(IntVector2 left, IntVector2 right)
    {
        return new IntVector2(left.x * right.x, left.y * right.y);
    }

    public static Vector2 operator *(IntVector2 left, Vector2 right)
    {
        return new Vector2(left.x * right.x, left.y * right.y);
    }

    public static Vector2 operator *(Vector2 left, IntVector2 right)
    {
        return new Vector2(left.x * right.x, left.y * right.y);
    }

    public static IntVector2 operator /(IntVector2 left, int factor)
    {
        return new IntVector2(left.x / factor, left.y / factor);
    }

    public static IntVector2 operator /(IntVector2 left, IntVector2 right)
    {
        return new IntVector2(left.x / right.x, left.y / right.y);
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
        return Tuple.Create(x, y).GetHashCode();
    }

    public override string ToString()
    {
        return String.Format("({0}, {1})", this.x.ToString(), this.y.ToString());
    }

    public string ToString(string format)
    {
        return String.Format("({0}, {1})", this.x.ToString(format), this.y.ToString(format));
    }

    public static explicit operator Vector2(IntVector2 v)
    {
        return new Vector2(v.x, v.y);
    }

    public float Length()
    {
        return Mathf.Sqrt(this.LengthSquared());
    }

    public float LengthSquared()
    {
        return x * x + y * y;
    }
}