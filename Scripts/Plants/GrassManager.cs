using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class GrassManager : PlantManager
{
    private static readonly byte GRASS_BLOCK_ID = Game.GetBlockId<GrassBlock>();
    private static readonly byte RED_ROCK_ID = Game.GetBlockId<RedRock>();
    private const byte AIR_ID = WorldGenerator.AIR_ID;

    private const float GRASS_DEATH_RATE = 5;

    private const float BASE_GAS_PRODUCTION = 0.00000001f;
    public static readonly IDictionary<Gas, float> GAS_PRODUCTION = new Dictionary<Gas, float>
    {
        [Gas.OXYGEN] = BASE_GAS_PRODUCTION,
        [Gas.NITROGEN] = -BASE_GAS_PRODUCTION,
        [Gas.CARBON_DIOXIDE] = -BASE_GAS_PRODUCTION,
    };
    public static readonly IDictionary<Gas, float> GAS_REQUIREMENTS = new Dictionary<Gas, float>
    {
        [Gas.OXYGEN] = 0.2f,
        [Gas.NITROGEN] = 0.8f,
        [Gas.CARBON_DIOXIDE] = 0.5f,
    };

    private const float MAX_GRASS_UPDATE_TIME = 1000 / 20f; // in msec

    // must be symmetric around (0,0,0)
    private static readonly IntVector3[] ADJACENT_BLOCK_VECTORS = new IntVector3[12] {
        new IntVector3(-1, -1, 0), new IntVector3(-1, 0, 0), new IntVector3(-1, 1, 0),
        new IntVector3(0, -1, -1), new IntVector3(0, 0, -1), new IntVector3(0, 1, -1),
        new IntVector3(0, -1, 1), new IntVector3(0, 0, 1), new IntVector3(0, 1, 1),
        new IntVector3(1, -1, 0), new IntVector3(1, 0, 0), new IntVector3(1, 1, 0)
    };

    private const double SPREAD_CHANCE = 0.65f;

    // TODO: introduce chunk locality
    private HashQueue<IntVector2> chunksToPlantGrassOn;
    private IDictionary<IntVector2, ISet<IntVector3>> grassToPlantGivenChunk;

    private float time;

    public GrassManager(Plants plants) : base(plants, SPREAD_CHANCE, GAS_PRODUCTION)
    {
        time = 0;
        chunksToPlantGrassOn = new HashQueue<IntVector2>();
        grassToPlantGivenChunk = new Dictionary<IntVector2, ISet<IntVector3>>();
    }

    private bool BlocksAlrightToSpread(IntVector3 blockPos)
    {
        return terrain.GetBlock(blockPos) == RED_ROCK_ID &&
               terrain.GetBlock(blockPos + new IntVector3(0, 1, 0)) == AIR_ID;
    }
    
    protected override bool CanGrowOn(IntVector3 blockPos)
    {
        return BlocksAlrightToSpread(blockPos) && 
            GAS_REQUIREMENTS.All(kvPair => atmosphere.GetGasProgress(kvPair.Key) >= kvPair.Value);
    }

    public override bool PlantOn(IntVector3 blockPos)
    {
        if (CanGrowOn(blockPos))
        {
            terrain.SetBlock(blockPos, GRASS_BLOCK_ID);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void PlantOnChunk()
    {
        IntVector2 chunk = chunksToPlantGrassOn.Dequeue();
        IEnumerable<IntVector3> validBlocks = grassToPlantGivenChunk[chunk].Where(CanGrowOn);
        IEnumerable<Tuple<IntVector3, byte>> blocksToChange =
            grassToPlantGivenChunk[chunk]
                .Where(CanGrowOn)
                .Select(pos => Tuple.Create(pos, GRASS_BLOCK_ID));
        grassToPlantGivenChunk.Remove(chunk);
        terrain.SetBlocks(blocksToChange);
    }

    public void UpdateActive(IntVector3 blockPos)
    {
        if (plantBlocks.Contains(blockPos))
        {
            if (ADJACENT_BLOCK_VECTORS.Any(delta => BlocksAlrightToSpread(delta+blockPos)))
            {
                plantActiveBlocks.Add(blockPos);
            }
            else
            {
                plantActiveBlocks.Remove(blockPos);
            }
        }
    }

    public void RespondToChangedGrassiness(IntVector3 blockPos)
    {
        UpdateActive(blockPos);
        foreach(IntVector3 delta in ADJACENT_BLOCK_VECTORS)
        {
            UpdateActive(blockPos + delta);
        }
    }

    public override void LifeCycle(float delta)
    {
        time += delta;
        if (time < LIFECYCLE_TICK_TIME || plantBlocks.Count == 0)
        {
            return;
        }
        time = 0;


        List<Tuple<IntVector3, byte>> grassThatDied = new List<Tuple<IntVector3, byte>>();

        // kill off some grass if there is too little gas
        float numberToDie = GRASS_DEATH_RATE * 
            GAS_REQUIREMENTS.Sum(kvPair => Mathf.Max(kvPair.Value - atmosphere.GetGasProgress(kvPair.Key), 0));

        while (numberToDie > 0 && plantBlocks.Count > 0)
        {
            if (numberToDie < 1 && randGen.NextDouble() > numberToDie)
                break;

            int idx = randGen.Next(plantBlocks.Count);
            IntVector3 block = plantBlocks.ElementAt(idx);
            DeregisterGrassAt(block);

            grassThatDied.Add(Tuple.Create(block, RED_ROCK_ID));
            numberToDie--;
        }
        terrain.SetBlocks(grassThatDied);

        Spread();
    }

    public void DeregisterGrassAt(IntVector3 position)
    {
        plantBlocks.Remove(position);
        plantActiveBlocks.Remove(position);
    }

    protected override void Spread()
    {
        // with small probability, pick random point and spread to adjacent block
        for (double spreadNo = plantActiveBlocks.Count * spreadChance; spreadNo > 0; spreadNo--)
        {
            if (spreadNo < 1 && randGen.NextDouble() > spreadNo)
                return;

            // find an active grass block
            IntVector3 block = plantActiveBlocks.ElementAt(randGen.Next(plantActiveBlocks.Count));

            // get adjacent blocks
            List<IntVector3> adjacentBlocks = new List<IntVector3>();
            foreach (IntVector3 v in ADJACENT_BLOCK_VECTORS)
            {
                if (BlocksAlrightToSpread(block + v))
                {
                    adjacentBlocks.Add(block + v);
                }
            }

            // choose an adjacent block and plant on it
            if (adjacentBlocks.Count > 0)
            {
                AddGrassToPlant(adjacentBlocks[randGen.Next(adjacentBlocks.Count)]);
            }
            else
            {
                throw new Exception("Active grass should always have adjacent blocks available to spread");
            }
        }
    }

    private void AddGrassToPlant(IntVector3 grassPos)
    {
        IntVector2 chunk = Terrain.ChunkIndex(grassPos);
        if (grassToPlantGivenChunk.TryGetValue(chunk, out ISet<IntVector3> grassPosSet))
        {
            grassPosSet.Add(grassPos);
        }
        else
        {
            grassToPlantGivenChunk[chunk] = new HashSet<IntVector3>
            {
                grassPos
            };
            chunksToPlantGrassOn.Enqueue(chunk);
        }
    }

    public override void HandleBlockChange(byte oldId, byte newId, IntVector3 blockPos)
    {
        // remove grass
        if (oldId == GRASS_BLOCK_ID && newId != GRASS_BLOCK_ID)
        {
            DeregisterGrassAt(blockPos);
            RespondToChangedGrassiness(blockPos);
        }
        
        // uncover soil
        if (oldId != AIR_ID && newId == AIR_ID)
        {
            if (blockPos.y > 0)
            {
                IntVector3 under = blockPos + new IntVector3(0, -1, 0);
                if (terrain.GetBlock(under) == RED_ROCK_ID)
                {
                    RespondToChangedGrassiness(under);
                }
            }
        }

        // add grass
        if (newId == GRASS_BLOCK_ID)
        {
            plantBlocks.Add(blockPos);
            RespondToChangedGrassiness(blockPos);
        }

        // add soil
        if (newId == RED_ROCK_ID)
        {
            RespondToChangedGrassiness(blockPos);
        }

        // cover something that was uncovered before
        if (oldId == AIR_ID && newId != AIR_ID && blockPos.y > 0)
        {
            IntVector3 under = blockPos + new IntVector3(0, -1, 0);
            byte underBlockId = terrain.GetBlock(under);

            if (underBlockId == GRASS_BLOCK_ID) // cover grass
            {
                terrain.SetBlock(under, RED_ROCK_ID);
            }
            else if (underBlockId == RED_ROCK_ID) // cover soil
            {
                RespondToChangedGrassiness(blockPos);
            }
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        float tStart = OS.GetTicksMsec();
        while (chunksToPlantGrassOn.Count > 0 && OS.GetTicksMsec() - tStart < MAX_GRASS_UPDATE_TIME)
        {
            PlantOnChunk();
        }
    }
}
