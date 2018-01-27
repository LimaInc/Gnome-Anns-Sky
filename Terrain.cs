using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Terrain : Spatial
{
    SurfaceTool surfaceTool = new SurfaceTool();

    Dictionary<IntVector2, Chunk> hardLoadedChunks = new Dictionary<IntVector2, Chunk>(); //Maybe we should just store an array of chunks?
    Dictionary<IntVector2, Chunk> softLoadedChunks = new Dictionary<IntVector2, Chunk>(); //Maybe we should just store an array of chunks?

    WorldGenerator worldGenerator = new WorldGenerator(); //Passed to chunks so they know how to generate their terrain

    //Creates a chunk at specified index, note that the chunk's position will be chunkIndex * chunkSize
    private Chunk CreateChunkAndHardLoad(IntVector2 chunkIndex)
    {
        if (!softLoadedChunks.ContainsKey(chunkIndex))
        {
            Chunk chunk = new Chunk(this,worldGenerator, chunkIndex);
            chunk.SoftLoad();
            chunk.HardLoad();
            this.AddChild(chunk);
            GD.Print("hit");
            softLoadedChunks[chunkIndex] = chunk;
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

    private void RemoveChunk(IntVector2 chunkIndex)
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
        IntVector2 chunkIndex = new IntVector2((int)Mathf.Floor((float)x / Chunk.CHUNK_SIZE.x),
                                               (int)Mathf.Floor((float)z / Chunk.CHUNK_SIZE.z));

        IntVector3 positionInChunk = new IntVector3(x,y,z) - (new IntVector3(chunkIndex.x, 0, chunkIndex.y) * Chunk.CHUNK_SIZE);

        Chunk chunk;
        if(softLoadedChunks.TryGetValue(chunkIndex, out chunk))
        {

            return chunk.GetBlockInChunk(positionInChunk);
        }
        else //Chunk isn't loaded, so return 0?
        {
            Chunk c = new Chunk(this,worldGenerator, chunkIndex);
            c.SoftLoad();

            softLoadedChunks.Add(chunkIndex, c);

            return c.GetBlockInChunk(positionInChunk);
        }
    }

    public override void _Ready()
    {
        player = GetNode("/root/Node/Player") as Player;
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
        Vector3 playerPos = player.GetPosition();
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

        GD.Print("Setting block " + pos + " to " + block + " in chunk " + chunkIndex);

        Chunk chunk = hardLoadedChunks[chunkIndex];

        IntVector3 positionInChunk = new IntVector3(pos.x - chunkIndex.x * Chunk.CHUNK_SIZE.x, pos.y, pos.z - chunkIndex.y * Chunk.CHUNK_SIZE.z);

        GD.Print("Position in chunk is " + positionInChunk);

        chunk.SetBlockInChunk(positionInChunk, block);

        IntVector2 right = chunkIndex + new IntVector2(1,0);
        IntVector2 left = chunkIndex + new IntVector2(-1,0);
        IntVector2 above = chunkIndex + new IntVector2(0,1);
        IntVector2 below = chunkIndex + new IntVector2(0,-1);

        if (positionInChunk.x == Chunk.CHUNK_SIZE.x - 1)
            hardLoadedChunks[right].UpdateMesh();

        if (positionInChunk.x == 0)
            hardLoadedChunks[left].UpdateMesh();

        if (positionInChunk.z == Chunk.CHUNK_SIZE.z - 1)
            hardLoadedChunks[above].UpdateMesh();

        if (positionInChunk.z == 0)
            hardLoadedChunks[below].UpdateMesh();

        chunk.UpdateMesh();
    }

    private void UpdateVisibleChunks()
    {
        Vector3 playerPos = player.GetPosition();

        GD.Print(playerPos);

        IntVector2 playerChunk = new IntVector2((int) (playerPos.x / Chunk.CHUNK_SIZE.x), (int) (playerPos.z / Chunk.CHUNK_SIZE.z));

        List<IntVector2> chunksLoadedThisUpdate = new List<IntVector2>();

        for (int x = playerChunk.x - chunkLoadRadius.x; x <= playerChunk.x + chunkLoadRadius.x; x++)
        {
            for (int z = playerChunk.y - chunkLoadRadius.y; z <= playerChunk.y + chunkLoadRadius.y; z++)
            {
                IntVector2 thisChunkIndex = new IntVector2(x,z);
                chunksLoadedThisUpdate.Add(thisChunkIndex);
                if(!hardLoadedChunks.ContainsKey(thisChunkIndex))
                    hardLoadedChunks[thisChunkIndex] = CreateChunkAndHardLoad(thisChunkIndex);
            }
        }

        hardLoadedChunks.Keys.Except(chunksLoadedThisUpdate).ToList().ForEach(x => RemoveChunk(x));
    }
}
