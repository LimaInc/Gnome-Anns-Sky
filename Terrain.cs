using Godot;
using System;
using System.Collections.Generic;

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

    public static implicit operator Vector3(IntVector3 vec)
    {
        return new Vector3(vec.x, vec.y, vec.z);
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

public class Terrain : Spatial
{
    SurfaceTool surfaceTool = new SurfaceTool();

    IntVector3 chunkSize = new IntVector3(16, 64, 16);

    Dictionary<IntVector3, Chunk> loadedChunks = new Dictionary<IntVector3, Chunk>(); //Maybe we should just store an array of chunks?

    WorldGenerator worldGenerator = new WorldGenerator(); //Passed to chunks so they know how to generate their terrain

    //Creates a chunk at specified index, note that the chunk's position will be chunkIndex * chunkSize
    private Chunk CreateChunk(IntVector3 chunkIndex)
    {
        Chunk chunk = new Chunk(worldGenerator, chunkIndex * chunkSize, chunkSize);
        this.AddChild(chunk);
        loadedChunks.Add(chunkIndex, chunk);
        return chunk;
    }

    public byte GetBlock(int x, int y, int z)
    {
        IntVector3 chunkIndex = new IntVector3(x / chunkSize.x, y / chunkSize.y, z / chunkSize.z);

        Chunk chunk;
        if(loadedChunks.TryGetValue(chunkIndex, out chunk))
        {
            return chunk.GetBlockInChunk(x % chunkSize.x, y % chunkSize.y, z % chunkSize.z);
        }
        else //Chunk isn't loaded, so return 0?
        {
            return 0;
        }
    }

    public override void _Ready()
    {
        for(int x = -4; x < 4; x++)
        {
            for(int y = 0; y < 1; y++)
            {
                for(int z = -4; z < 4; z++)
                {
                    CreateChunk(new IntVector3(x,y,z));
                }
            }
        }
    }

    public override void _Process(float delta)
    {
        // Called every frame. Delta is time since last frame.
        // Update game logic here.
        
        
    }
}
