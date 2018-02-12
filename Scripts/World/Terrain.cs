using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Terrain : Spatial
{
    private Dictionary<IntVector2, Chunk> loadedChunks = new Dictionary<IntVector2, Chunk>();

    public WorldGenerator worldGenerator = new WorldGenerator();

    //Creates a chunk at specified index, note that the chunk's position will be chunkIndex * chunkSize
    private Chunk CreateChunk(IntVector2 chunkIndex)
    {
        if(loadedChunks.ContainsKey(chunkIndex))
            return loadedChunks[chunkIndex];
        
        byte[,,] blocks = worldGenerator.GetChunk(chunkIndex.x, chunkIndex.y, Chunk.CHUNK_SIZE.x, Chunk.CHUNK_SIZE.y, Chunk.CHUNK_SIZE.z);
        Chunk chunk = new Chunk(this, chunkIndex, blocks);
        this.AddChild(chunk);

        return chunk;
    }

    private void RemoveChunk(IntVector2 chunkIndex)
    {
        Chunk chunk = loadedChunks[chunkIndex];
        chunk.Visible = false;
        chunk.QueueFree();
        loadedChunks.Remove(chunkIndex);
    }

    public byte GetBlock(IntVector3 v)
    {
        return GetBlock(v.x, v.y, v.z);
    }

    public byte GetBlock(int x, int y, int z)
    {
        //Messy code here is because C# rounds integer division towards 0, rather than negative infinity like we want :(
        IntVector2 chunkIndex = new IntVector2((int)Mathf.Floor((float)x / Chunk.CHUNK_SIZE.x),
                                               (int)Mathf.Floor((float)z / Chunk.CHUNK_SIZE.z));

        IntVector3 positionInChunk = new IntVector3(x,y,z) - (new IntVector3(chunkIndex.x, 0, chunkIndex.y) * Chunk.CHUNK_SIZE);

        Chunk chunk;
        if(loadedChunks.TryGetValue(chunkIndex, out chunk))
            return chunk.GetBlockInChunk(positionInChunk);
        else //Should only happen when outside chunks are checking for adjacent blocks
        {
            return 255;
        }
    }

    public override void _Ready()
    {
        player = GetNode("/root/Game/Player") as Player;
    }

    Player player;
    int chunkLoadRadius = 8;

    Vector3 playerPosLastUpdate = new Vector3(-50, -50, -50); //Forces update on first frame
    float updateDistance = 10;
    public override void _Process(float delta)
    {  
        //Update visible chunks only when the player has moved a certain distance
        Vector3 playerPos = player.GetTranslation();
        if((playerPos - playerPosLastUpdate).LengthSquared() > (updateDistance * updateDistance))
        {
            playerPosLastUpdate = playerPos;
            UpdateVisibleChunks();
        }

        if(chunksToUpdate.Count > 0)
        {
            loadedChunks[chunksToUpdate.Dequeue()].UpdateMesh();
        }
    }

    public void SetBlock(IntVector3 pos, byte block)
    {
        //Messy code here is because C# rounds integer division towards 0, rather than negative infinity like we want :(
        IntVector2 chunkIndex = new IntVector2((int)Mathf.Floor((float)pos.x / Chunk.CHUNK_SIZE.x),
                                               (int)Mathf.Floor((float)pos.z / Chunk.CHUNK_SIZE.z));

        Chunk chunk = loadedChunks[chunkIndex];

        IntVector3 positionInChunk = new IntVector3(pos.x - chunkIndex.x * Chunk.CHUNK_SIZE.x, pos.y, pos.z - chunkIndex.y * Chunk.CHUNK_SIZE.z);

        chunk.SetBlockInChunk(positionInChunk, block);

        IntVector2 right = chunkIndex + new IntVector2(1,0);
        IntVector2 left = chunkIndex + new IntVector2(-1,0);
        IntVector2 above = chunkIndex + new IntVector2(0,1);
        IntVector2 below = chunkIndex + new IntVector2(0,-1);

        chunksToUpdate.Enqueue(chunkIndex);

        if (positionInChunk.x == Chunk.CHUNK_SIZE.x - 1)
            chunksToUpdate.Enqueue(right);

        if (positionInChunk.x == 0)
            chunksToUpdate.Enqueue(left);

        if (positionInChunk.z == Chunk.CHUNK_SIZE.z - 1)
            chunksToUpdate.Enqueue(above);

        if (positionInChunk.z == 0)
            chunksToUpdate.Enqueue(below);
    }

    Queue<IntVector2> chunksToUpdate = new Queue<IntVector2>();

    //Generated from https://stackoverflow.com/a/9591896
    IntVector2[][] chunkLoadIndices = new IntVector2[][]
    {
        new IntVector2[] { new IntVector2(0,0) },
        new IntVector2[] { new IntVector2(-1, 0), new IntVector2( 0, 1), new IntVector2( 1, 0), new IntVector2( 0,-1) },
        new IntVector2[] { new IntVector2(-1,-1), new IntVector2(-1, 1), new IntVector2( 1, 1), new IntVector2( 1,-1) },
        new IntVector2[] { new IntVector2(-2, 0), new IntVector2( 0, 2), new IntVector2( 2, 0), new IntVector2( 0,-2) },
        new IntVector2[] { new IntVector2(-1,-2), new IntVector2(-2,-1), new IntVector2(-2, 1), new IntVector2(-1, 2), new IntVector2( 1, 2), new IntVector2( 2, 1), new IntVector2( 2,-1), new IntVector2( 1,-2) },
        new IntVector2[] { new IntVector2(-2,-2), new IntVector2(-2, 2), new IntVector2( 2, 2), new IntVector2( 2,-2) },
        new IntVector2[] { new IntVector2(-3, 0), new IntVector2( 0, 3), new IntVector2( 3, 0), new IntVector2( 0,-3) },
        new IntVector2[] { new IntVector2(-1,-3), new IntVector2(-3,-1), new IntVector2(-3, 1), new IntVector2(-1, 3), new IntVector2( 1, 3), new IntVector2( 3, 1), new IntVector2( 3,-1), new IntVector2( 1,-3) },
        new IntVector2[] { new IntVector2(-2,-3), new IntVector2(-3,-2), new IntVector2(-3, 2), new IntVector2(-2, 3), new IntVector2( 2, 3), new IntVector2( 3, 2), new IntVector2( 3,-2), new IntVector2( 2,-3) },
        new IntVector2[] { new IntVector2(-4, 0), new IntVector2( 0, 4), new IntVector2( 4, 0), new IntVector2( 0,-4) },
        new IntVector2[] { new IntVector2(-1,-4), new IntVector2(-4,-1), new IntVector2(-4, 1), new IntVector2(-1, 4), new IntVector2( 1, 4), new IntVector2( 4, 1), new IntVector2( 4,-1), new IntVector2( 1,-4) },
        new IntVector2[] { new IntVector2(-3,-3), new IntVector2(-3, 3), new IntVector2( 3, 3), new IntVector2( 3,-3) },
        new IntVector2[] { new IntVector2(-2,-4), new IntVector2(-4,-2), new IntVector2(-4, 2), new IntVector2(-2, 4), new IntVector2( 2, 4), new IntVector2( 4, 2), new IntVector2( 4,-2), new IntVector2( 2,-4) },
        new IntVector2[] { new IntVector2(-3,-4), new IntVector2(-4,-3), new IntVector2(-5, 0), new IntVector2(-4, 3), new IntVector2(-3, 4), new IntVector2( 0, 5), new IntVector2( 3, 4), new IntVector2( 4, 3), new IntVector2( 5, 0), new IntVector2( 4,-3), new IntVector2( 3,-4), new IntVector2( 0,-5) },
        new IntVector2[] { new IntVector2(-1,-5), new IntVector2(-5,-1), new IntVector2(-5, 1), new IntVector2(-1, 5), new IntVector2( 1, 5), new IntVector2( 5, 1), new IntVector2( 5,-1), new IntVector2( 1,-5) },
        new IntVector2[] { new IntVector2(-2,-5), new IntVector2(-5,-2), new IntVector2(-5, 2), new IntVector2(-2, 5), new IntVector2( 2, 5), new IntVector2( 5, 2), new IntVector2( 5,-2), new IntVector2( 2,-5) },
        new IntVector2[] { new IntVector2(-4,-4), new IntVector2(-4, 4), new IntVector2( 4, 4), new IntVector2( 4,-4) },
        new IntVector2[] { new IntVector2(-3,-5), new IntVector2(-5,-3), new IntVector2(-5, 3), new IntVector2(-3, 5), new IntVector2( 3, 5), new IntVector2( 5, 3), new IntVector2( 5,-3), new IntVector2( 3,-5) },
        new IntVector2[] { new IntVector2(-6, 0), new IntVector2( 0, 6), new IntVector2( 6, 0), new IntVector2( 0,-6) },
        new IntVector2[] { new IntVector2(-1,-6), new IntVector2(-6,-1), new IntVector2(-6, 1), new IntVector2(-1, 6), new IntVector2( 1, 6), new IntVector2( 6, 1), new IntVector2( 6,-1), new IntVector2( 1,-6) },
        new IntVector2[] { new IntVector2(-2,-6), new IntVector2(-6,-2), new IntVector2(-6, 2), new IntVector2(-2, 6), new IntVector2( 2, 6), new IntVector2( 6, 2), new IntVector2( 6,-2), new IntVector2( 2,-6) },
        new IntVector2[] { new IntVector2(-4,-5), new IntVector2(-5,-4), new IntVector2(-5, 4), new IntVector2(-4, 5), new IntVector2( 4, 5), new IntVector2( 5, 4), new IntVector2( 5,-4), new IntVector2( 4,-5) },
        new IntVector2[] { new IntVector2(-3,-6), new IntVector2(-6,-3), new IntVector2(-6, 3), new IntVector2(-3, 6), new IntVector2( 3, 6), new IntVector2( 6, 3), new IntVector2( 6,-3), new IntVector2( 3,-6) },
        new IntVector2[] { new IntVector2(-7, 0), new IntVector2( 0, 7), new IntVector2( 7, 0), new IntVector2( 0,-7) },
        new IntVector2[] { new IntVector2(-1,-7), new IntVector2(-5,-5), new IntVector2(-7,-1), new IntVector2(-7, 1), new IntVector2(-5, 5), new IntVector2(-1, 7), new IntVector2( 1, 7), new IntVector2( 5, 5), new IntVector2( 7, 1), new IntVector2( 7,-1), new IntVector2( 5,-5), new IntVector2( 1,-7) },
        new IntVector2[] { new IntVector2(-4,-6), new IntVector2(-6,-4), new IntVector2(-6, 4), new IntVector2(-4, 6), new IntVector2( 4, 6), new IntVector2( 6, 4), new IntVector2( 6,-4), new IntVector2( 4,-6) },
        new IntVector2[] { new IntVector2(-2,-7), new IntVector2(-7,-2), new IntVector2(-7, 2), new IntVector2(-2, 7), new IntVector2( 2, 7), new IntVector2( 7, 2), new IntVector2( 7,-2), new IntVector2( 2,-7) },
        new IntVector2[] { new IntVector2(-3,-7), new IntVector2(-7,-3), new IntVector2(-7, 3), new IntVector2(-3, 7), new IntVector2( 3, 7), new IntVector2( 7, 3), new IntVector2( 7,-3), new IntVector2( 3,-7) },
        new IntVector2[] { new IntVector2(-5,-6), new IntVector2(-6,-5), new IntVector2(-6, 5), new IntVector2(-5, 6), new IntVector2( 5, 6), new IntVector2( 6, 5), new IntVector2( 6,-5), new IntVector2( 5,-6) },
        new IntVector2[] { new IntVector2(-8, 0), new IntVector2( 0, 8), new IntVector2( 8, 0), new IntVector2( 0,-8) },
        new IntVector2[] { new IntVector2(-1,-8), new IntVector2(-4,-7), new IntVector2(-7,-4), new IntVector2(-8,-1), new IntVector2(-8, 1), new IntVector2(-7, 4), new IntVector2(-4, 7), new IntVector2(-1, 8), new IntVector2( 1, 8), new IntVector2( 4, 7), new IntVector2( 7, 4), new IntVector2( 8, 1), new IntVector2( 8,-1), new IntVector2( 7,-4), new IntVector2( 4,-7), new IntVector2( 1,-8) },
        new IntVector2[] { new IntVector2(-2,-8), new IntVector2(-8,-2), new IntVector2(-8, 2), new IntVector2(-2, 8), new IntVector2( 2, 8), new IntVector2( 8, 2), new IntVector2( 8,-2), new IntVector2( 2,-8) },
        new IntVector2[] { new IntVector2(-6,-6), new IntVector2(-6, 6), new IntVector2( 6, 6), new IntVector2( 6,-6) },
        new IntVector2[] { new IntVector2(-3,-8), new IntVector2(-8,-3), new IntVector2(-8, 3), new IntVector2(-3, 8), new IntVector2( 3, 8), new IntVector2( 8, 3), new IntVector2( 8,-3), new IntVector2( 3,-8) },
        new IntVector2[] { new IntVector2(-5,-7), new IntVector2(-7,-5), new IntVector2(-7, 5), new IntVector2(-5, 7), new IntVector2( 5, 7), new IntVector2( 7, 5), new IntVector2( 7,-5), new IntVector2( 5,-7) },
        new IntVector2[] { new IntVector2(-4,-8), new IntVector2(-8,-4), new IntVector2(-8, 4), new IntVector2(-4, 8), new IntVector2( 4, 8), new IntVector2( 8, 4), new IntVector2( 8,-4), new IntVector2( 4,-8) },
        new IntVector2[] { new IntVector2(-9, 0), new IntVector2( 0, 9), new IntVector2( 9, 0), new IntVector2( 0,-9) },
        new IntVector2[] { new IntVector2(-1,-9), new IntVector2(-9,-1), new IntVector2(-9, 1), new IntVector2(-1, 9), new IntVector2( 1, 9), new IntVector2( 9, 1), new IntVector2( 9,-1), new IntVector2( 1,-9) },
        new IntVector2[] { new IntVector2(-2,-9), new IntVector2(-6,-7), new IntVector2(-7,-6), new IntVector2(-9,-2), new IntVector2(-9, 2), new IntVector2(-7, 6), new IntVector2(-6, 7), new IntVector2(-2, 9), new IntVector2( 2, 9), new IntVector2( 6, 7), new IntVector2( 7, 6), new IntVector2( 9, 2), new IntVector2( 9,-2), new IntVector2( 7,-6), new IntVector2( 6,-7), new IntVector2( 2,-9) },
        new IntVector2[] { new IntVector2(-5,-8), new IntVector2(-8,-5), new IntVector2(-8, 5), new IntVector2(-5, 8), new IntVector2( 5, 8), new IntVector2( 8, 5), new IntVector2( 8,-5), new IntVector2( 5,-8) },
        new IntVector2[] { new IntVector2(-3,-9), new IntVector2(-9,-3), new IntVector2(-9, 3), new IntVector2(-3, 9), new IntVector2( 3, 9), new IntVector2( 9, 3), new IntVector2( 9,-3), new IntVector2( 3,-9) },
        new IntVector2[] { new IntVector2(-4,-9), new IntVector2(-9,-4), new IntVector2(-9, 4), new IntVector2(-4, 9), new IntVector2( 4, 9), new IntVector2( 9, 4), new IntVector2( 9,-4), new IntVector2( 4,-9) },
        new IntVector2[] { new IntVector2(-7,-7), new IntVector2(-7, 7), new IntVector2( 7, 7), new IntVector2( 7,-7) },
        new IntVector2[] { new IntVector2( -6, -8), new IntVector2( -8, -6), new IntVector2(-10,  0), new IntVector2( -8,  6), new IntVector2( -6,  8), new IntVector2(  0, 10), new IntVector2(  6,  8), new IntVector2(  8,  6), new IntVector2( 10,  0), new IntVector2(  8, -6), new IntVector2(  6, -8), new IntVector2(  0,-10) },
        new IntVector2[] { new IntVector2( -1,-10), new IntVector2(-10, -1), new IntVector2(-10,  1), new IntVector2( -1, 10), new IntVector2(  1, 10), new IntVector2( 10,  1), new IntVector2( 10, -1), new IntVector2(  1,-10) },
        new IntVector2[] { new IntVector2( -2,-10), new IntVector2(-10, -2), new IntVector2(-10,  2), new IntVector2( -2, 10), new IntVector2(  2, 10), new IntVector2( 10,  2), new IntVector2( 10, -2), new IntVector2(  2,-10) },
        new IntVector2[] { new IntVector2(-5,-9), new IntVector2(-9,-5), new IntVector2(-9, 5), new IntVector2(-5, 9), new IntVector2( 5, 9), new IntVector2( 9, 5), new IntVector2( 9,-5), new IntVector2( 5,-9) },
        new IntVector2[] { new IntVector2( -3,-10), new IntVector2(-10, -3), new IntVector2(-10,  3), new IntVector2( -3, 10), new IntVector2(  3, 10), new IntVector2( 10,  3), new IntVector2( 10, -3), new IntVector2(  3,-10) },
        new IntVector2[] { new IntVector2(-7,-8), new IntVector2(-8,-7), new IntVector2(-8, 7), new IntVector2(-7, 8), new IntVector2( 7, 8), new IntVector2( 8, 7), new IntVector2( 8,-7), new IntVector2( 7,-8) },
        new IntVector2[] { new IntVector2( -4,-10), new IntVector2(-10, -4), new IntVector2(-10,  4), new IntVector2( -4, 10), new IntVector2(  4, 10), new IntVector2( 10,  4), new IntVector2( 10, -4), new IntVector2(  4,-10) },
        new IntVector2[] { new IntVector2(-6,-9), new IntVector2(-9,-6), new IntVector2(-9, 6), new IntVector2(-6, 9), new IntVector2( 6, 9), new IntVector2( 9, 6), new IntVector2( 9,-6), new IntVector2( 6,-9) },
        new IntVector2[] { new IntVector2(-11,  0), new IntVector2(  0, 11), new IntVector2( 11,  0), new IntVector2(  0,-11) },
        new IntVector2[] { new IntVector2( -1,-11), new IntVector2(-11, -1), new IntVector2(-11,  1), new IntVector2( -1, 11), new IntVector2(  1, 11), new IntVector2( 11,  1), new IntVector2( 11, -1), new IntVector2(  1,-11) },
        new IntVector2[] { new IntVector2( -2,-11), new IntVector2( -5,-10), new IntVector2(-10, -5), new IntVector2(-11, -2), new IntVector2(-11,  2), new IntVector2(-10,  5), new IntVector2( -5, 10), new IntVector2( -2, 11), new IntVector2(  2, 11), new IntVector2(  5, 10), new IntVector2( 10,  5), new IntVector2( 11,  2), new IntVector2( 11, -2), new IntVector2( 10, -5), new IntVector2(  5,-10), new IntVector2(  2,-11) },
        new IntVector2[] { new IntVector2(-8,-8), new IntVector2(-8, 8), new IntVector2( 8, 8), new IntVector2( 8,-8) },
        new IntVector2[] { new IntVector2( -3,-11), new IntVector2( -7, -9), new IntVector2( -9, -7), new IntVector2(-11, -3), new IntVector2(-11,  3), new IntVector2( -9,  7), new IntVector2( -7,  9), new IntVector2( -3, 11), new IntVector2(  3, 11), new IntVector2(  7,  9), new IntVector2(  9,  7), new IntVector2( 11,  3), new IntVector2( 11, -3), new IntVector2(  9, -7), new IntVector2(  7, -9), new IntVector2(  3,-11) },
        new IntVector2[] { new IntVector2( -6,-10), new IntVector2(-10, -6), new IntVector2(-10,  6), new IntVector2( -6, 10), new IntVector2(  6, 10), new IntVector2( 10,  6), new IntVector2( 10, -6), new IntVector2(  6,-10) },
        new IntVector2[] { new IntVector2( -4,-11), new IntVector2(-11, -4), new IntVector2(-11,  4), new IntVector2( -4, 11), new IntVector2(  4, 11), new IntVector2( 11,  4), new IntVector2( 11, -4), new IntVector2(  4,-11) },
        new IntVector2[] { new IntVector2(-12,  0), new IntVector2(  0, 12), new IntVector2( 12,  0), new IntVector2(  0,-12) },
        new IntVector2[] { new IntVector2( -1,-12), new IntVector2( -8, -9), new IntVector2( -9, -8), new IntVector2(-12, -1), new IntVector2(-12,  1), new IntVector2( -9,  8), new IntVector2( -8,  9), new IntVector2( -1, 12), new IntVector2(  1, 12), new IntVector2(  8,  9), new IntVector2(  9,  8), new IntVector2( 12,  1), new IntVector2( 12, -1), new IntVector2(  9, -8), new IntVector2(  8, -9), new IntVector2(  1,-12) },
        new IntVector2[] { new IntVector2( -5,-11), new IntVector2(-11, -5), new IntVector2(-11,  5), new IntVector2( -5, 11), new IntVector2(  5, 11), new IntVector2( 11,  5), new IntVector2( 11, -5), new IntVector2(  5,-11) },
        new IntVector2[] { new IntVector2( -2,-12), new IntVector2(-12, -2), new IntVector2(-12,  2), new IntVector2( -2, 12), new IntVector2(  2, 12), new IntVector2( 12,  2), new IntVector2( 12, -2), new IntVector2(  2,-12) },
        new IntVector2[] { new IntVector2( -7,-10), new IntVector2(-10, -7), new IntVector2(-10,  7), new IntVector2( -7, 10), new IntVector2(  7, 10), new IntVector2( 10,  7), new IntVector2( 10, -7), new IntVector2(  7,-10) },
        new IntVector2[] { new IntVector2( -3,-12), new IntVector2(-12, -3), new IntVector2(-12,  3), new IntVector2( -3, 12), new IntVector2(  3, 12), new IntVector2( 12,  3), new IntVector2( 12, -3), new IntVector2(  3,-12) },
        new IntVector2[] { new IntVector2( -6,-11), new IntVector2(-11, -6), new IntVector2(-11,  6), new IntVector2( -6, 11), new IntVector2(  6, 11), new IntVector2( 11,  6), new IntVector2( 11, -6), new IntVector2(  6,-11) },
        new IntVector2[] { new IntVector2( -4,-12), new IntVector2(-12, -4), new IntVector2(-12,  4), new IntVector2( -4, 12), new IntVector2(  4, 12), new IntVector2( 12,  4), new IntVector2( 12, -4), new IntVector2(  4,-12) },
        new IntVector2[] { new IntVector2(-9,-9), new IntVector2(-9, 9), new IntVector2( 9, 9), new IntVector2( 9,-9) },
        new IntVector2[] { new IntVector2( -8,-10), new IntVector2(-10, -8), new IntVector2(-10,  8), new IntVector2( -8, 10), new IntVector2(  8, 10), new IntVector2( 10,  8), new IntVector2( 10, -8), new IntVector2(  8,-10) },
        new IntVector2[] { new IntVector2( -5,-12), new IntVector2(-12, -5), new IntVector2(-13,  0), new IntVector2(-12,  5), new IntVector2( -5, 12), new IntVector2(  0, 13), new IntVector2(  5, 12), new IntVector2( 12,  5), new IntVector2( 13,  0), new IntVector2( 12, -5), new IntVector2(  5,-12), new IntVector2(  0,-13) },
        new IntVector2[] { new IntVector2( -1,-13), new IntVector2( -7,-11), new IntVector2(-11, -7), new IntVector2(-13, -1), new IntVector2(-13,  1), new IntVector2(-11,  7), new IntVector2( -7, 11), new IntVector2( -1, 13), new IntVector2(  1, 13), new IntVector2(  7, 11), new IntVector2( 11,  7), new IntVector2( 13,  1), new IntVector2( 13, -1), new IntVector2( 11, -7), new IntVector2(  7,-11), new IntVector2(  1,-13) },
        new IntVector2[] { new IntVector2( -2,-13), new IntVector2(-13, -2), new IntVector2(-13,  2), new IntVector2( -2, 13), new IntVector2(  2, 13), new IntVector2( 13,  2), new IntVector2( 13, -2), new IntVector2(  2,-13) },
        new IntVector2[] { new IntVector2( -3,-13), new IntVector2(-13, -3), new IntVector2(-13,  3), new IntVector2( -3, 13), new IntVector2(  3, 13), new IntVector2( 13,  3), new IntVector2( 13, -3), new IntVector2(  3,-13) },
        new IntVector2[] { new IntVector2( -6,-12), new IntVector2(-12, -6), new IntVector2(-12,  6), new IntVector2( -6, 12), new IntVector2(  6, 12), new IntVector2( 12,  6), new IntVector2( 12, -6), new IntVector2(  6,-12) },
        new IntVector2[] { new IntVector2( -9,-10), new IntVector2(-10, -9), new IntVector2(-10,  9), new IntVector2( -9, 10), new IntVector2(  9, 10), new IntVector2( 10,  9), new IntVector2( 10, -9), new IntVector2(  9,-10) },
        new IntVector2[] { new IntVector2( -4,-13), new IntVector2( -8,-11), new IntVector2(-11, -8), new IntVector2(-13, -4), new IntVector2(-13,  4), new IntVector2(-11,  8), new IntVector2( -8, 11), new IntVector2( -4, 13), new IntVector2(  4, 13), new IntVector2(  8, 11), new IntVector2( 11,  8), new IntVector2( 13,  4), new IntVector2( 13, -4), new IntVector2( 11, -8), new IntVector2(  8,-11), new IntVector2(  4,-13) },
        new IntVector2[] { new IntVector2( -7,-12), new IntVector2(-12, -7), new IntVector2(-12,  7), new IntVector2( -7, 12), new IntVector2(  7, 12), new IntVector2( 12,  7), new IntVector2( 12, -7), new IntVector2(  7,-12) },
        new IntVector2[] { new IntVector2( -5,-13), new IntVector2(-13, -5), new IntVector2(-13,  5), new IntVector2( -5, 13), new IntVector2(  5, 13), new IntVector2( 13,  5), new IntVector2( 13, -5), new IntVector2(  5,-13) },
        new IntVector2[] { new IntVector2(-14,  0), new IntVector2(  0, 14), new IntVector2( 14,  0), new IntVector2(  0,-14) },
        new IntVector2[] { new IntVector2( -1,-14), new IntVector2(-14, -1), new IntVector2(-14,  1), new IntVector2( -1, 14), new IntVector2(  1, 14), new IntVector2( 14,  1), new IntVector2( 14, -1), new IntVector2(  1,-14) },
        new IntVector2[] { new IntVector2( -2,-14), new IntVector2(-10,-10), new IntVector2(-14, -2), new IntVector2(-14,  2), new IntVector2(-10, 10), new IntVector2( -2, 14), new IntVector2(  2, 14), new IntVector2( 10, 10), new IntVector2( 14,  2), new IntVector2( 14, -2), new IntVector2( 10,-10), new IntVector2(  2,-14) },
        new IntVector2[] { new IntVector2( -9,-11), new IntVector2(-11, -9), new IntVector2(-11,  9), new IntVector2( -9, 11), new IntVector2(  9, 11), new IntVector2( 11,  9), new IntVector2( 11, -9), new IntVector2(  9,-11) },
        new IntVector2[] { new IntVector2( -3,-14), new IntVector2( -6,-13), new IntVector2(-13, -6), new IntVector2(-14, -3), new IntVector2(-14,  3), new IntVector2(-13,  6), new IntVector2( -6, 13), new IntVector2( -3, 14), new IntVector2(  3, 14), new IntVector2(  6, 13), new IntVector2( 13,  6), new IntVector2( 14,  3), new IntVector2( 14, -3), new IntVector2( 13, -6), new IntVector2(  6,-13), new IntVector2(  3,-14) },
        new IntVector2[] { new IntVector2( -8,-12), new IntVector2(-12, -8), new IntVector2(-12,  8), new IntVector2( -8, 12), new IntVector2(  8, 12), new IntVector2( 12,  8), new IntVector2( 12, -8), new IntVector2(  8,-12) },
        new IntVector2[] { new IntVector2( -4,-14), new IntVector2(-14, -4), new IntVector2(-14,  4), new IntVector2( -4, 14), new IntVector2(  4, 14), new IntVector2( 14,  4), new IntVector2( 14, -4), new IntVector2(  4,-14) },
        new IntVector2[] { new IntVector2( -7,-13), new IntVector2(-13, -7), new IntVector2(-13,  7), new IntVector2( -7, 13), new IntVector2(  7, 13), new IntVector2( 13,  7), new IntVector2( 13, -7), new IntVector2(  7,-13) },
        new IntVector2[] { new IntVector2( -5,-14), new IntVector2(-10,-11), new IntVector2(-11,-10), new IntVector2(-14, -5), new IntVector2(-14,  5), new IntVector2(-11, 10), new IntVector2(-10, 11), new IntVector2( -5, 14), new IntVector2(  5, 14), new IntVector2( 10, 11), new IntVector2( 11, 10), new IntVector2( 14,  5), new IntVector2( 14, -5), new IntVector2( 11,-10), new IntVector2( 10,-11), new IntVector2(  5,-14) },
        new IntVector2[] { new IntVector2( -9,-12), new IntVector2(-12, -9), new IntVector2(-15,  0), new IntVector2(-12,  9), new IntVector2( -9, 12), new IntVector2(  0, 15), new IntVector2(  9, 12), new IntVector2( 12,  9), new IntVector2( 15,  0), new IntVector2( 12, -9), new IntVector2(  9,-12), new IntVector2(  0,-15) },
        new IntVector2[] { new IntVector2( -1,-15), new IntVector2(-15, -1), new IntVector2(-15,  1), new IntVector2( -1, 15), new IntVector2(  1, 15), new IntVector2( 15,  1), new IntVector2( 15, -1), new IntVector2(  1,-15) },
        new IntVector2[] { new IntVector2( -2,-15), new IntVector2(-15, -2), new IntVector2(-15,  2), new IntVector2( -2, 15), new IntVector2(  2, 15), new IntVector2( 15,  2), new IntVector2( 15, -2), new IntVector2(  2,-15) },
        new IntVector2[] { new IntVector2( -6,-14), new IntVector2(-14, -6), new IntVector2(-14,  6), new IntVector2( -6, 14), new IntVector2(  6, 14), new IntVector2( 14,  6), new IntVector2( 14, -6), new IntVector2(  6,-14) },
        new IntVector2[] { new IntVector2( -8,-13), new IntVector2(-13, -8), new IntVector2(-13,  8), new IntVector2( -8, 13), new IntVector2(  8, 13), new IntVector2( 13,  8), new IntVector2( 13, -8), new IntVector2(  8,-13) },
        new IntVector2[] { new IntVector2( -3,-15), new IntVector2(-15, -3), new IntVector2(-15,  3), new IntVector2( -3, 15), new IntVector2(  3, 15), new IntVector2( 15,  3), new IntVector2( 15, -3), new IntVector2(  3,-15) },
        new IntVector2[] { new IntVector2( -4,-15), new IntVector2(-15, -4), new IntVector2(-15,  4), new IntVector2( -4, 15), new IntVector2(  4, 15), new IntVector2( 15,  4), new IntVector2( 15, -4), new IntVector2(  4,-15) },
        new IntVector2[] { new IntVector2(-11,-11), new IntVector2(-11, 11), new IntVector2( 11, 11), new IntVector2( 11,-11) },
        new IntVector2[] { new IntVector2(-10,-12), new IntVector2(-12,-10), new IntVector2(-12, 10), new IntVector2(-10, 12), new IntVector2( 10, 12), new IntVector2( 12, 10), new IntVector2( 12,-10), new IntVector2( 10,-12) },
        new IntVector2[] { new IntVector2( -7,-14), new IntVector2(-14, -7), new IntVector2(-14,  7), new IntVector2( -7, 14), new IntVector2(  7, 14), new IntVector2( 14,  7), new IntVector2( 14, -7), new IntVector2(  7,-14) },
        new IntVector2[] { new IntVector2( -5,-15), new IntVector2( -9,-13), new IntVector2(-13, -9), new IntVector2(-15, -5), new IntVector2(-15,  5), new IntVector2(-13,  9), new IntVector2( -9, 13), new IntVector2( -5, 15), new IntVector2(  5, 15), new IntVector2(  9, 13), new IntVector2( 13,  9), new IntVector2( 15,  5), new IntVector2( 15, -5), new IntVector2( 13, -9), new IntVector2(  9,-13), new IntVector2(  5,-15) },
    };
    
    private void UpdateVisibleChunks()
    {
        Vector3 playerPos = player.GetTranslation();

        IntVector2 playerChunk = new IntVector2((int) (playerPos.x / (Chunk.CHUNK_SIZE.x * Chunk.BLOCK_SIZE)), (int) (playerPos.z / (Chunk.CHUNK_SIZE.z * Chunk.BLOCK_SIZE)));

        List<IntVector2> chunksLoadedThisUpdate = new List<IntVector2>();

        for(int r = 0; r < chunkLoadRadius * 2; r++)
        {
            foreach(IntVector2 v in chunkLoadIndices[r])
            {
                IntVector2 thisChunkIndex = playerChunk + v;
                chunksLoadedThisUpdate.Add(thisChunkIndex);
                if(!loadedChunks.ContainsKey(thisChunkIndex))
                {
                    loadedChunks[thisChunkIndex] = CreateChunk(thisChunkIndex);
                    chunksToUpdate.Enqueue(thisChunkIndex);
                }
            }
        }

        //newChunks.ToList().ForEach(x => loadedChunks[x].UpdateMesh());

        loadedChunks.Keys.Except(chunksLoadedThisUpdate).ToList().ForEach(x => RemoveChunk(x));
    }
}
