using System;
using Godot;

public class Vector4 : IEquatable<Vector4> //Used for chunk/block positions
{
    public float x;
    public float y;
    public float z;
    public float w;

    public Vector4(float x, float y, float z, float w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public static Vector4 operator +(Vector4 left, Vector4 right)
    {
        left.x += right.x;
        left.y += right.y;
        left.z += right.z;
        left.w += right.w;
        return left;
    }

    public static Vector4 operator -(Vector4 left, Vector4 right)
    {
        left.x -= right.x;
        left.y -= right.y;
        left.z -= right.z;
        left.w -= left.w;
        return left;
    }

    public static Vector4 operator -(Vector4 vec)
    {
        vec.x = -vec.x;
        vec.y = -vec.y;
        vec.z = -vec.z;
        vec.w = -vec.w;
        return vec;
    }

    public static Vector4 operator *(Vector4 left, Vector4 right)
    {
        left.x *= right.x;
        left.y *= right.y;
        left.z *= right.z;
        left.w *= right.w;
        return left;
    }

    public static Vector4 operator *(Vector4 left, float right)
    {
        return new Vector4(left.x * right, left.y * right, left.z * right, left.w * right);
    }

    public static Vector4 operator /(Vector4 left, float right)
    {
        return left * (1 / right);
    }

    public static implicit operator Vector3(Vector4 vec)
    {
        return new Vector3(vec.x, vec.y, vec.z);
    }

    public static bool operator ==(Vector4 left, Vector4 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Vector4 left, Vector4 right)
    {
        return !left.Equals(right);
    }

    public override bool Equals(object obj)
    {
        if (obj is Vector4)
        {
            return Equals((Vector4)obj);
        }

        return false;
    }

    public bool Equals(Vector4 other)
    {
        return x == other.x && y == other.y && z == other.z && w == other.w;
    }

    public override int GetHashCode()
    {
        return x.GetHashCode() + 31 * (y.GetHashCode() + 31 * (z.GetHashCode() + 31 * w.GetHashCode()));
    }

    public override string ToString()
    {
        return String.Format("({0}, {1}, {2}, {3})", this.x, this.y, this.z, this.w);
    }

    public string ToString(string format)
    {
        return String.Format("({0}, {1}, {2}, {3})", 
            this.x.ToString(format), 
            this.y.ToString(format), 
            this.z.ToString(format), 
            this.w.ToString(format));
    }
}