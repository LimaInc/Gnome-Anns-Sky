using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Terrain : Spatial
{
    public const int AVERAGE_HEIGHT = 55;
    public const int HEIGHT_SPREAD = 128;
    public const int SEA_LEVEL = 30;
    public const int RED_ROCK_LAYER_NUM = 3;
    public const int FOSSIL_DEPTH = RED_ROCK_LAYER_NUM;

    public const float BASE_FOSSIL_SPAWN_RATE = Game.FOSSIL_SPAWN_MULITPLIER * 0.0003f;

    public readonly IList<UniformRandomBlockGenerator> fossilGenerators = new List<UniformRandomBlockGenerator>()
    {
        new UniformRandomBlockGenerator(Game.GetBlockId<FrogFossilBlock>(), FOSSIL_DEPTH, BASE_FOSSIL_SPAWN_RATE * 2),
        new UniformRandomBlockGenerator(Game.GetBlockId<RegularAnimalFossilBlock>(), FOSSIL_DEPTH, BASE_FOSSIL_SPAWN_RATE / 2),
        new UniformRandomBlockGenerator(Game.GetBlockId<BigAnimalFossilBlock>(), FOSSIL_DEPTH, BASE_FOSSIL_SPAWN_RATE),
        new UniformRandomBlockGenerator(Game.GetBlockId<GrassFossilBlock>(), FOSSIL_DEPTH, BASE_FOSSIL_SPAWN_RATE * 3),
        new UniformRandomBlockGenerator(Game.GetBlockId<TreeFossilBlock>(), FOSSIL_DEPTH, BASE_FOSSIL_SPAWN_RATE * 1.5f ),
        new UniformRandomBlockGenerator(Game.GetBlockId<CarbonDioxideBacteriaFossilBlock>(), FOSSIL_DEPTH, BASE_FOSSIL_SPAWN_RATE * 10),
        new UniformRandomBlockGenerator(Game.GetBlockId<OxygenBacteriaFossilBlock>(), FOSSIL_DEPTH, BASE_FOSSIL_SPAWN_RATE * 4),
        new UniformRandomBlockGenerator(Game.GetBlockId<NitrogenBacteriaFossilBlock>(), FOSSIL_DEPTH, BASE_FOSSIL_SPAWN_RATE)
    };

    //Stores the loaded chunks, indexed by their position, whether chunk model is currently loaded and whether the node exists in the Godot scene currently
    private IDictionary<IntVector2, Tuple<Chunk, bool, bool>> loadedChunks = new Dictionary<IntVector2, Tuple<Chunk, bool, bool>>();

    public WorldGenerator worldGenerator;

    public override void _Ready()
    {
        player = GetNode(Game.PLAYER_PATH) as Player;

        worldGenerator = new WorldGenerator();
        worldGenerator.terrainModifiers.Add(new HillLandscapeTM(AVERAGE_HEIGHT, HEIGHT_SPREAD));
        worldGenerator.generators.Add(new IceGenerator(SEA_LEVEL));
        worldGenerator.generators.Add(new RockGenerator(RED_ROCK_LAYER_NUM));

        foreach (UniformRandomBlockGenerator g in fossilGenerators)
        {
            worldGenerator.generators.Add(g);
        }

        Base b = GetNode(Game.PLANET_BASE_PATH) as Base;
        Vector2 horizontalBasePos = new Vector2(b.position.x, b.position.z);
        b.position = new IntVector3(b.position.x, worldGenerator.GetHeightAt(horizontalBasePos) - 1, b.position.z);
        b.InitGeneration();
        worldGenerator.terrainModifiers.Add(b.Smoothing);
        worldGenerator.generators.Add(b.Generator);

        Vector2 playerPos = new Vector2(player.Translation.x, player.Translation.z) / Block.SIZE;
        float terrainGraphicalHeight = worldGenerator.GetHeightAt(playerPos) * Block.SIZE;
        player.Translation = new Vector3(playerPos.x, terrainGraphicalHeight + Player.INIT_REL_POS.y, playerPos.y);
    }

    public const float ANIMAL_CHUNK_RANGE = 16.0f;

    private int chunkNo = 0;

    private Dictionary<string,int> CountAnimalsInChunk(IntVector2 chunkIndex)
    {
        Vector3 playerPos = player.GetTranslation();

        Vector3 chunkCentre = (new Vector3(chunkIndex.x * Chunk.CHUNK_SIZE.x, Chunk.CHUNK_SIZE.y / 2.0f, chunkIndex.y * Chunk.CHUNK_SIZE.z) * Chunk.BLOCK_SIZE) + (new Vector3((Chunk.CHUNK_SIZE.x * Chunk.BLOCK_SIZE), 0, (Chunk.CHUNK_SIZE.y * Chunk.BLOCK_SIZE)) / 2.0f);      

        Array bodies = ((Area)player.GetNode("AnimalArea")).GetOverlappingBodies();

        Dictionary<string, int> count = new Dictionary<string, int>();

        foreach (PhysicsBody body in bodies)
        {
            if (body.IsInGroup("animals"))
            {
                AnimalBehaviourComponent behaviour = ((Entity)body.GetNode("Entity")).GetComponent<AnimalBehaviourComponent>();
                if (!count.ContainsKey(behaviour.PresetName))
                {
                    count.Add(behaviour.PresetName,1);
                }
                else
                {
                    count[behaviour.PresetName]++;
                }
            }
        }

        return count;
    }

    //Creates a chunk at specified index, note that the chunk's position will be chunkIndex * chunkSize
    private void CreateChunk(IntVector2 chunkIndex, bool buildMesh)
    {
        if (loadedChunks.TryGetValue(chunkIndex, out Tuple<Chunk, bool, bool> tuple)) //Chunk already created
        {
            if (buildMesh && !tuple.Item2) //But maybe we need to build a mesh for it?
            {
                loadedChunks[chunkIndex] = new Tuple<Chunk, bool, bool>(tuple.Item1, true, tuple.Item3);
                chunksToUpdate.Enqueue(chunkIndex);
            }

            if (!tuple.Item3) // If node not created, but it is in memory
            {
                tuple.Item1.Visible = true;
            }
        }
        else
        {
            byte[,,] blocks = worldGenerator.GetChunk(chunkIndex, Chunk.SIZE);
            Chunk chunk = new Chunk(this, chunkIndex, blocks);
            this.AddChild(chunk);
            loadedChunks[chunkIndex] = new Tuple<Chunk, bool, bool>(chunk, buildMesh, true);
            if (buildMesh)
                chunksToUpdate.Enqueue(chunkIndex);
        }

        chunkNo++;

        //We take data from ANIMAL_CHUNK_RANGE closest chunks to player, throw it in a normal distribution, and generate animals from the dist.
        //We can't handle so many animals, though, so currently only generate animals in one in every ANIMAL_CHUNK_RANGE * 4 chunks (so 1/4 the expected amount).
        if(chunkNo == (int)ANIMAL_CHUNK_RANGE * 4)
        {
            chunkNo = 0;
            // Spawn animals
            Vector3 playerPos = player.GetTranslation();
            IntVector2 playerChunk = new IntVector2((int)(playerPos.x / (Chunk.CHUNK_SIZE.x * Chunk.BLOCK_SIZE)), (int)(playerPos.z / (Chunk.CHUNK_SIZE.z * Chunk.BLOCK_SIZE)));
            Dictionary<string, int> animalCount = CountAnimalsInChunk(playerChunk);

            Random rand = new Random();

            foreach (KeyValuePair<string, int> pair in animalCount)
            {
                //number to spawn
                double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
                double u2 = 1.0 - rand.NextDouble();
                double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                             Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
                double randNormal = (pair.Value + (pair.Value / 4.0f) * randStdNormal);

                int number = (int)(Math.Max(0, Math.Round(randNormal / ANIMAL_CHUNK_RANGE)));

                for (int i = 0; i < number; i++)
                {
                    double sexNum = rand.NextDouble();
                    AnimalBehaviourComponent.AnimalSex sex = (sexNum > 0.5 ? AnimalBehaviourComponent.AnimalSex.Male : AnimalBehaviourComponent.AnimalSex.Female);
                    Vector3 chunkOrigin = (new Vector3(chunkIndex.x * Chunk.CHUNK_SIZE.x, Chunk.CHUNK_SIZE.y / 2.0f, chunkIndex.y * Chunk.CHUNK_SIZE.z) * Chunk.BLOCK_SIZE);
                    Vector3 chunkSize = Chunk.CHUNK_SIZE * Chunk.BLOCK_SIZE;
                    Vector3 randomPosition = new Vector3(rand.Next(0, (int)chunkSize.x), 100.0f, rand.Next(0, (int)chunkSize.z));
                    GetTree().GetRoot().GetNode("Game").GetNode("AnimalSpawner").Call("SpawnAnimal", pair.Key, sex, chunkOrigin+randomPosition);
                }
            }
        }
    }

    private void RemoveChunk(IntVector2 chunkIndex)
    {
        Chunk chunk = loadedChunks[chunkIndex].Item1;
        chunk.Visible = false;
        //chunk.QueueFree();
        loadedChunks[chunkIndex] = new Tuple<Chunk, bool, bool>(chunk, false, false);
        //loadedChunks.Remove(chunkIndex); // Can't be fucked to implement proper saving/loading, so lets just keep them all in memory >:)
    }

    public byte GetBlock(IntVector3 v)
    {
        return GetBlock(v.x, v.y, v.z);
    }

    public byte GetBlock(int x, int y, int z)
    {
        IntVector2 chunkIndex = new IntVector2(MathUtil.RoundDownDiv(x, Chunk.SIZE.x),
                                               MathUtil.RoundDownDiv(z, Chunk.SIZE.z));

        IntVector3 positionInChunk = new IntVector3(x,y,z) - (new IntVector3(chunkIndex.x, 0, chunkIndex.y) * Chunk.SIZE);

        if (loadedChunks.TryGetValue(chunkIndex, out Tuple<Chunk, bool, bool> tuple))
        {
            return tuple.Item1.GetBlockInChunk(positionInChunk);
        }
        else // Should only happen when outside chunks are checking for adjacent blocks
        {
            return Byte.MaxValue;
        }
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
            loadedChunks[chunksToUpdate.Dequeue()].Item1.UpdateMesh();
        }
        else if(chunksToRemove.Count > 0)
        {
            RemoveChunk(chunksToRemove.Dequeue());
        }
    }

    public void SetBlocks(Tuple<IntVector3, byte>[] blocks)
    {
        HashSet<IntVector2> chunks = new HashSet<IntVector2>();
        foreach (Tuple<IntVector3, byte> b in blocks)
        {
            IntVector3 pos = b.Item1;
            byte block = b.Item2;

            //Messy code here is because C# rounds integer division towards 0, rather than negative infinity like we want :(
            IntVector2 chunkIndex = new IntVector2((int)Mathf.Floor((float)pos.x / Chunk.SIZE.x),
                                                   (int)Mathf.Floor((float)pos.z / Chunk.SIZE.z));

            Chunk chunk = loadedChunks[chunkIndex].Item1;

            IntVector3 positionInChunk = new IntVector3(pos.x - chunkIndex.x * Chunk.SIZE.x, pos.y, pos.z - chunkIndex.y * Chunk.SIZE.z);

            chunk.TrySetBlockInChunk(positionInChunk, block);

            IntVector2 right = chunkIndex + new IntVector2(1,0);
            IntVector2 left = chunkIndex + new IntVector2(-1,0);
            IntVector2 above = chunkIndex + new IntVector2(0,1);
            IntVector2 below = chunkIndex + new IntVector2(0,-1);

            chunks.Add(chunkIndex);

            if (positionInChunk.x == Chunk.SIZE.x - 1)
                chunks.Add(right);

            if (positionInChunk.x == 0)
                chunks.Add(left);

            if (positionInChunk.z == Chunk.SIZE.z - 1)
                chunks.Add(above);

            if (positionInChunk.z == 0)
                chunks.Add(below);
        }

        foreach (IntVector2 chunk in chunks)
            chunksToUpdate.Enqueue(chunk);
    }

    public void SetBlock(IntVector3 pos, byte block)
    {
        //Messy code here is because C# rounds integer division towards 0, rather than negative infinity like we want :(
        IntVector2 chunkIndex = new IntVector2((int)Mathf.Floor((float)pos.x / Chunk.SIZE.x),
                                               (int)Mathf.Floor((float)pos.z / Chunk.SIZE.z));

        Chunk chunk = loadedChunks[chunkIndex].Item1;

        IntVector3 positionInChunk = new IntVector3(pos.x - chunkIndex.x * Chunk.SIZE.x, pos.y, pos.z - chunkIndex.y * Chunk.SIZE.z);

        chunk.TrySetBlockInChunk(positionInChunk, block);

        IntVector2 right = chunkIndex + new IntVector2(1,0);
        IntVector2 left = chunkIndex + new IntVector2(-1,0);
        IntVector2 above = chunkIndex + new IntVector2(0,1);
        IntVector2 below = chunkIndex + new IntVector2(0,-1);

        chunksToUpdate.Enqueue(chunkIndex);

        if (positionInChunk.x == Chunk.SIZE.x - 1)
            chunksToUpdate.Enqueue(right);

        if (positionInChunk.x == 0)
            chunksToUpdate.Enqueue(left);

        if (positionInChunk.z == Chunk.SIZE.z - 1)
            chunksToUpdate.Enqueue(above);

        if (positionInChunk.z == 0)
            chunksToUpdate.Enqueue(below);
    }

    //Queue<IntVector2> chunksToUpdate = new Queue<IntVector2>();
    //Queue<IntVector2> chunksToRemove = new Queue<IntVector2>();

    HashQueue<IntVector2> chunksToUpdate = new HashQueue<IntVector2>();
    HashQueue<IntVector2> chunksToRemove = new HashQueue<IntVector2>();

    // WTF ???
    // TODO: fix, definitely, urgently
    IntVector2[][] chunkLoadIndices = new IntVector2[][]
    {
        new IntVector2[] { new IntVector2(0, 0), },
        new IntVector2[] { new IntVector2(0, 1), new IntVector2(-1, 0), new IntVector2(1, 0), new IntVector2(0, -1), },
        new IntVector2[] { new IntVector2(0, 2), new IntVector2(-1, 1), new IntVector2(1, 1), new IntVector2(-2, 0), new IntVector2(-1, -1), new IntVector2(2, 0), new IntVector2(1, -1), new IntVector2(0, -2), },
        new IntVector2[] { new IntVector2(0, 3), new IntVector2(-1, 2), new IntVector2(1, 2), new IntVector2(-2, 1), new IntVector2(2, 1), new IntVector2(-3, 0), new IntVector2(-2, -1), new IntVector2(-1, -2), new IntVector2(3, 0), new IntVector2(2, -1), new IntVector2(1, -2), new IntVector2(0, -3), },
        new IntVector2[] { new IntVector2(0, 4), new IntVector2(-1, 3), new IntVector2(1, 3), new IntVector2(-2, 2), new IntVector2(2, 2), new IntVector2(-3, 1), new IntVector2(3, 1), new IntVector2(-4, 0), new IntVector2(-3, -1), new IntVector2(-2, -2), new IntVector2(-1, -3), new IntVector2(4, 0), new IntVector2(3, -1), new IntVector2(2, -2), new IntVector2(1, -3), new IntVector2(0, -4), },
        new IntVector2[] { new IntVector2(0, 5), new IntVector2(-1, 4), new IntVector2(1, 4), new IntVector2(-2, 3), new IntVector2(2, 3), new IntVector2(-3, 2), new IntVector2(3, 2), new IntVector2(-4, 1), new IntVector2(4, 1), new IntVector2(-5, 0), new IntVector2(-4, -1), new IntVector2(-3, -2), new IntVector2(-2, -3), new IntVector2(-1, -4), new IntVector2(5, 0), new IntVector2(4, -1), new IntVector2(3, -2), new IntVector2(2, -3), new IntVector2(1, -4), new IntVector2(0, -5), },
        new IntVector2[] { new IntVector2(0, 6), new IntVector2(-1, 5), new IntVector2(1, 5), new IntVector2(-2, 4), new IntVector2(2, 4), new IntVector2(-3, 3), new IntVector2(3, 3), new IntVector2(-4, 2), new IntVector2(4, 2), new IntVector2(-5, 1), new IntVector2(5, 1), new IntVector2(-6, 0), new IntVector2(-5, -1), new IntVector2(-4, -2), new IntVector2(-3, -3), new IntVector2(-2, -4), new IntVector2(-1, -5), new IntVector2(6, 0), new IntVector2(5, -1), new IntVector2(4, -2), new IntVector2(3, -3), new IntVector2(2, -4), new IntVector2(1, -5), new IntVector2(0, -6), },
        new IntVector2[] { new IntVector2(0, 7), new IntVector2(-1, 6), new IntVector2(1, 6), new IntVector2(-2, 5), new IntVector2(2, 5), new IntVector2(-3, 4), new IntVector2(3, 4), new IntVector2(-4, 3), new IntVector2(4, 3), new IntVector2(-5, 2), new IntVector2(5, 2), new IntVector2(-6, 1), new IntVector2(6, 1), new IntVector2(-7, 0), new IntVector2(-6, -1), new IntVector2(-5, -2), new IntVector2(-4, -3), new IntVector2(-3, -4), new IntVector2(-2, -5), new IntVector2(-1, -6), new IntVector2(7, 0), new IntVector2(6, -1), new IntVector2(5, -2), new IntVector2(4, -3), new IntVector2(3, -4), new IntVector2(2, -5), new IntVector2(1, -6), new IntVector2(0, -7), },
        new IntVector2[] { new IntVector2(0, 8), new IntVector2(-1, 7), new IntVector2(1, 7), new IntVector2(-2, 6), new IntVector2(2, 6), new IntVector2(-3, 5), new IntVector2(3, 5), new IntVector2(-4, 4), new IntVector2(4, 4), new IntVector2(-5, 3), new IntVector2(5, 3), new IntVector2(-6, 2), new IntVector2(6, 2), new IntVector2(-7, 1), new IntVector2(7, 1), new IntVector2(-8, 0), new IntVector2(-7, -1), new IntVector2(-6, -2), new IntVector2(-5, -3), new IntVector2(-4, -4), new IntVector2(-3, -5), new IntVector2(-2, -6), new IntVector2(-1, -7), new IntVector2(8, 0), new IntVector2(7, -1), new IntVector2(6, -2), new IntVector2(5, -3), new IntVector2(4, -4), new IntVector2(3, -5), new IntVector2(2, -6), new IntVector2(1, -7), new IntVector2(0, -8), },
        new IntVector2[] { new IntVector2(0, 9), new IntVector2(-1, 8), new IntVector2(1, 8), new IntVector2(-2, 7), new IntVector2(2, 7), new IntVector2(-3, 6), new IntVector2(3, 6), new IntVector2(-4, 5), new IntVector2(4, 5), new IntVector2(-5, 4), new IntVector2(5, 4), new IntVector2(-6, 3), new IntVector2(6, 3), new IntVector2(-7, 2), new IntVector2(7, 2), new IntVector2(-8, 1), new IntVector2(8, 1), new IntVector2(-9, 0), new IntVector2(-8, -1), new IntVector2(-7, -2), new IntVector2(-6, -3), new IntVector2(-5, -4), new IntVector2(-4, -5), new IntVector2(-3, -6), new IntVector2(-2, -7), new IntVector2(-1, -8), new IntVector2(9, 0), new IntVector2(8, -1), new IntVector2(7, -2), new IntVector2(6, -3), new IntVector2(5, -4), new IntVector2(4, -5), new IntVector2(3, -6), new IntVector2(2, -7), new IntVector2(1, -8), new IntVector2(0, -9), },
        new IntVector2[] { new IntVector2(0, 10), new IntVector2(-1, 9), new IntVector2(1, 9), new IntVector2(-2, 8), new IntVector2(2, 8), new IntVector2(-3, 7), new IntVector2(3, 7), new IntVector2(-4, 6), new IntVector2(4, 6), new IntVector2(-5, 5), new IntVector2(5, 5), new IntVector2(-6, 4), new IntVector2(6, 4), new IntVector2(-7, 3), new IntVector2(7, 3), new IntVector2(-8, 2), new IntVector2(8, 2), new IntVector2(-9, 1), new IntVector2(9, 1), new IntVector2(-10, 0), new IntVector2(-9, -1), new IntVector2(-8, -2), new IntVector2(-7, -3), new IntVector2(-6, -4), new IntVector2(-5, -5), new IntVector2(-4, -6), new IntVector2(-3, -7), new IntVector2(-2, -8), new IntVector2(-1, -9), new IntVector2(10, 0), new IntVector2(9, -1), new IntVector2(8, -2), new IntVector2(7, -3), new IntVector2(6, -4), new IntVector2(5, -5), new IntVector2(4, -6), new IntVector2(3, -7), new IntVector2(2, -8), new IntVector2(1, -9), new IntVector2(0, -10), },
        new IntVector2[] { new IntVector2(0, 11), new IntVector2(-1, 10), new IntVector2(1, 10), new IntVector2(-2, 9), new IntVector2(2, 9), new IntVector2(-3, 8), new IntVector2(3, 8), new IntVector2(-4, 7), new IntVector2(4, 7), new IntVector2(-5, 6), new IntVector2(5, 6), new IntVector2(-6, 5), new IntVector2(6, 5), new IntVector2(-7, 4), new IntVector2(7, 4), new IntVector2(-8, 3), new IntVector2(8, 3), new IntVector2(-9, 2), new IntVector2(9, 2), new IntVector2(-10, 1), new IntVector2(10, 1), new IntVector2(-11, 0), new IntVector2(-10, -1), new IntVector2(-9, -2), new IntVector2(-8, -3), new IntVector2(-7, -4), new IntVector2(-6, -5), new IntVector2(-5, -6), new IntVector2(-4, -7), new IntVector2(-3, -8), new IntVector2(-2, -9), new IntVector2(-1, -10), new IntVector2(11, 0), new IntVector2(10, -1), new IntVector2(9, -2), new IntVector2(8, -3), new IntVector2(7, -4), new IntVector2(6, -5), new IntVector2(5, -6), new IntVector2(4, -7), new IntVector2(3, -8), new IntVector2(2, -9), new IntVector2(1, -10), new IntVector2(0, -11), },
   };
    
    private void UpdateVisibleChunks()
    {
        Vector3 playerPos = player.GetTranslation();

        IntVector2 playerChunk = new IntVector2((int) (playerPos.x / (Chunk.SIZE.x * Block.SIZE)), (int) (playerPos.z / (Chunk.SIZE.z * Block.SIZE)));

        List<IntVector2> chunksLoadedThisUpdate = new List<IntVector2>();

        for(int r = 0; r < chunkLoadRadius; r++)
        {
            foreach(IntVector2 v in chunkLoadIndices[r])
            {
                IntVector2 thisChunkIndex = playerChunk + v;
                chunksLoadedThisUpdate.Add(thisChunkIndex);
                CreateChunk(thisChunkIndex, true);
            }
        }

        //Load an extra ring of chunks, but don't build meshes for them
        for(int r = chunkLoadRadius; r < chunkLoadRadius + 1; r++)
        {
            foreach(IntVector2 v in chunkLoadIndices[r])
            {
                IntVector2 thisChunkIndex = playerChunk + v;
                chunksLoadedThisUpdate.Add(thisChunkIndex);
                CreateChunk(thisChunkIndex, false);
            }
        }

        chunksToRemove = new HashQueue<IntVector2>(loadedChunks.Keys.Except(chunksLoadedThisUpdate));
    }
}
