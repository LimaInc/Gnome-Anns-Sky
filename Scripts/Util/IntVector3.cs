using System;
using Godot;

public struct IntVector3 : IEquatable<IntVector3> //Used for chunk/block positions
{
    public int x;
    public int y;
    public int z;

    public IntVector3(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static IntVector3 operator +(IntVector3 left, IntVector3 right)
    {
        left.x += right.x;
        left.y += right.y;
        left.z += right.z;
        return left;
    }

    public static IntVector3 operator -(IntVector3 left, IntVector3 right)
    {
        left.x -= right.x;
        left.y -= right.y;
        left.z -= right.z;
        return left;
    }

    public static IntVector3 operator -(IntVector3 vec)
    {
        vec.x = -vec.x;
        vec.y = -vec.y;
        vec.z = -vec.z;
        return vec;
    }

    public static IntVector3 operator *(IntVector3 left, IntVector3 right)
    {
        left.x *= right.x;
        left.y *= right.y;
        left.z *= right.z;
        return left;
    }

    public static Vector3 operator *(IntVector3 left, float right)
    {
        Vector3 vec;
        vec.x = left.x * right;
        vec.y = left.y * right;
        vec.z = left.z * right;
        return vec;
    }

    public static implicit operator Vector3(IntVector3 vec)
    {
        return new Vector3(vec.x, vec.y, vec.z);
    }

    public static explicit operator IntVector3(Vector3 vec)
    {
        return new IntVector3((int)vec.x, (int)vec.y, (int)vec.z);
    }

    public static bool operator ==(IntVector3 left, IntVector3 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(IntVector3 left, IntVector3 right)
    {
        return !left.Equals(right);
    }

    public override bool Equals(object obj)
    {
        if(obj is IntVector3)
        {
            return Equals((IntVector3)obj);
        }

        return false;
    }

    public bool Equals(IntVector3 other)
    {
        return x == other.x && y == other.y && z == other.z;
    }

    public override int GetHashCode()
    {
        return y.GetHashCode() ^ x.GetHashCode() ^ z.GetHashCode();
    }

    public override string ToString()
    {
        return String.Format("({0}, {1}, {2})", this.x.ToString(), this.y.ToString(), this.z.ToString());
    }

    public string ToString(string format)
    {
        return String.Format("({0}, {1}, {2})", this.x.ToString(format), this.y.ToString(format), this.z.ToString(format));
    }
}