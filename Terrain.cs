using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

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

    Dictionary<IntVector3, Chunk> hardLoadedChunks = new Dictionary<IntVector3, Chunk>(); //Maybe we should just store an array of chunks?
    Dictionary<IntVector3, Chunk> softLoadedChunks = new Dictionary<IntVector3, Chunk>(); //Maybe we should just store an array of chunks?

    WorldGenerator worldGenerator = new WorldGenerator(); //Passed to chunks so they know how to generate their terrain

    //Creates a chunk at specified index, note that the chunk's position will be chunkIndex * chunkSize
    private Chunk CreateChunkAndHardLoad(IntVector3 chunkIndex)
    {
        if (!softLoadedChunks.ContainsKey(chunkIndex))
        {
            Chunk chunk = new Chunk(this,worldGenerator, chunkIndex, Chunk.CHUNK_SIZE);
            chunk.SoftLoad();
            chunk.HardLoad();
            this.AddChild(chunk);
            softLoadedChunks.Add(chunkIndex, chunk);
            hardLoadedChunks.Add(chunkIndex, chunk);
            return chunk;
        } else 
        {
            Chunk c = softLoadedChunks[chunkIndex];
            c.HardLoad();
            this.AddChild(c);
            hardLoadedChunks.Add(chunkIndex, c);
            return c;
        }
    }

    private void RemoveChunk(IntVector3 chunkIndex)
    {
        Chunk chunk = hardLoadedChunks[chunkIndex];
        chunk.Visible = false;
        chunk.QueueFree();
        softLoadedChunks.Remove(chunkIndex);
        hardLoadedChunks.Remove(chunkIndex);
    }

    public byte GetBlock(int x, int y, int z)
    {
        //Messy code here is because C# rounds integer division towards 0, rather than negative infinity like we want :(
        IntVector3 chunkIndex = new IntVector3((int)Mathf.Floor((float)x / Chunk.CHUNK_SIZE.x),
                                               (int)Mathf.Floor((float)y / Chunk.CHUNK_SIZE.y),
                                               (int)Mathf.Floor((float)z / Chunk.CHUNK_SIZE.z));

        IntVector3 positionInChunk = new IntVector3(x,y,z) - chunkIndex * Chunk.CHUNK_SIZE;

        Chunk chunk;
        if(softLoadedChunks.TryGetValue(chunkIndex, out chunk))
        {

            return chunk.GetBlockInChunk(positionInChunk);
        }
        else //Chunk isn't loaded, so return 0?
        {
            Chunk c = new Chunk(this,worldGenerator, chunkIndex * Chunk.CHUNK_SIZE, Chunk.CHUNK_SIZE);
            c.SoftLoad();

            softLoadedChunks.Add(chunkIndex, c);

            return c.GetBlockInChunk(positionInChunk);
        }
    }

    public override void _Ready()
    {
        player = GetNode("/root/Node/Player") as Spatial;
    }

    Spatial player;
    IntVector3 chunkLoadRadius = new IntVector3(8, 1, 8);

    Vector3 playerPosLastUpdate = new Vector3(-50, -50, -50); //Forces update on first frame
    float updateDistance = 10;
    public override void _Process(float delta)
    {
        // Called every frame. Delta is time since last frame.
        // Update game logic here.
        
        //Update visible chunks only when the player has moved a certain distance
        Vector3 playerPos = player.Translation;
        if((playerPos - playerPosLastUpdate).LengthSquared() > (updateDistance * updateDistance))
        {
            playerPosLastUpdate = playerPos;
            UpdateVisibleChunks();
        }
    }

    public void SetBlock(IntVector3 pos, byte block)
    {
        //Messy code here is because C# rounds integer division towards 0, rather than negative infinity like we want :(
        IntVector3 chunkIndex = new IntVector3((int)Mathf.Floor((float)pos.x / Chunk.CHUNK_SIZE.x),
                                               (int)Mathf.Floor((float)pos.y / Chunk.CHUNK_SIZE.y),
                                               (int)Mathf.Floor((float)pos.z / Chunk.CHUNK_SIZE.z));

        GD.Print("Setting block " + pos + " to " + block + " in chunk " + chunkIndex);

        Chunk chunk = hardLoadedChunks[chunkIndex];

        IntVector3 positionInChunk = pos - chunkIndex * Chunk.CHUNK_SIZE;

        GD.Print("Position in chunk is " + positionInChunk);

        chunk.SetBlockInChunk(positionInChunk, block);

        chunk.UpdateMesh();
    }

    private void UpdateVisibleChunks()
    {
        Vector3 playerPos = player.Translation;

        IntVector3 playerChunk = new IntVector3((int) (playerPos.x / Chunk.CHUNK_SIZE.x), (int) (playerPos.y / Chunk.CHUNK_SIZE.y), (int) (playerPos.z / Chunk.CHUNK_SIZE.z));

        List<IntVector3> chunksLoadedThisUpdate = new List<IntVector3>();

        for (int x = playerChunk.x - chunkLoadRadius.x; x <= playerChunk.x + chunkLoadRadius.x; x++)
        {
            for (int y = playerChunk.y - chunkLoadRadius.y; y <= playerChunk.y + chunkLoadRadius.y; y++)
            {
                for (int z = playerChunk.z - chunkLoadRadius.z; z <= playerChunk.z + chunkLoadRadius.z; z++)
                {
                    IntVector3 thisChunkIndex = new IntVector3(x,y,z);
                    chunksLoadedThisUpdate.Add(thisChunkIndex);
                    if(!hardLoadedChunks.ContainsKey(thisChunkIndex))
                        hardLoadedChunks[thisChunkIndex] = CreateChunkAndHardLoad(thisChunkIndex);
                    
                }
            }
        }

        hardLoadedChunks.Keys.Except(chunksLoadedThisUpdate).ToList().ForEach(x => RemoveChunk(x));
    }
}
