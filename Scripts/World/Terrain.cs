using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Terrain : Spatial
{
    private Dictionary<IntVector2, Chunk> loadedChunks = new Dictionary<IntVector2, Chunk>();

    private WorldGenerator worldGenerator = new WorldGenerator();

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
            return 1; //Just return 1 so they don't generate that face
    }

    public override void _Ready()
    {
        player = GetNode("/root/Game/Player") as Player;
    }

    Player player;
    IntVector2 chunkLoadRadius = new IntVector2(8, 8);

    Vector3 playerPosLastUpdate = new Vector3(-50, -50, -50); //Forces update on first frame
    float updateDistance = 10;
    public override void _Process(float delta)
    {
        // Called every frame. Delta is time since last frame.
        // Update game logic here.
        
        //Update visible chunks only when the player has moved a certain distance
        Vector3 playerPos = player.GetTranslation();
        if((playerPos - playerPosLastUpdate).LengthSquared() > (updateDistance * updateDistance))
        {
            playerPosLastUpdate = playerPos;
            UpdateVisibleChunks();
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

        if (positionInChunk.x == Chunk.CHUNK_SIZE.x - 1)
            loadedChunks[right].UpdateMesh();

        if (positionInChunk.x == 0)
            loadedChunks[left].UpdateMesh();

        if (positionInChunk.z == Chunk.CHUNK_SIZE.z - 1)
            loadedChunks[above].UpdateMesh();

        if (positionInChunk.z == 0)
            loadedChunks[below].UpdateMesh();

        chunk.UpdateMesh();
    }

    private void UpdateVisibleChunks()
    {
        Vector3 playerPos = player.GetTranslation();

        IntVector2 playerChunk = new IntVector2((int) (playerPos.x / (Chunk.CHUNK_SIZE.x * Chunk.BLOCK_SIZE)), (int) (playerPos.z / (Chunk.CHUNK_SIZE.z * Chunk.BLOCK_SIZE)));

        List<IntVector2> chunksLoadedThisUpdate = new List<IntVector2>();
        List<IntVector2> newChunks = new List<IntVector2>();

        for (int x = playerChunk.x - chunkLoadRadius.x; x <= playerChunk.x + chunkLoadRadius.x; x++)
        {
            for (int z = playerChunk.y - chunkLoadRadius.y; z <= playerChunk.y + chunkLoadRadius.y; z++)
            {
                IntVector2 thisChunkIndex = new IntVector2(x,z);
                chunksLoadedThisUpdate.Add(thisChunkIndex);
                if(!loadedChunks.ContainsKey(thisChunkIndex))
                {
                    loadedChunks[thisChunkIndex] = CreateChunk(thisChunkIndex);
                    newChunks.Add(thisChunkIndex);
                }
            }
        }

        newChunks.ToList().ForEach(x => loadedChunks[x].UpdateMesh());

        loadedChunks.Keys.Except(chunksLoadedThisUpdate).ToList().ForEach(x => RemoveChunk(x));
    }
}
