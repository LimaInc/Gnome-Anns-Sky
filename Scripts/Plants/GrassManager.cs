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

    private static IntVector3[] adjacentBlockVectors = new IntVector3[12] {
        new IntVector3(-1, -1, 0), new IntVector3(-1, 0, 0), new IntVector3(-1, 1, 0),
        new IntVector3(0, -1, -1), new IntVector3(0, 0, -1), new IntVector3(0, 1, -1),
        new IntVector3(0, -1, 1), new IntVector3(0, 0, 1), new IntVector3(0, 1, 1),
        new IntVector3(1, -1, 0), new IntVector3(1, 0, 0), new IntVector3(1, 1, 0)
    };

    private const double SPREAD_CHANCE = 0.65f;

    private float time;

    public GrassManager(Plants plants) : base(plants, SPREAD_CHANCE, GAS_PRODUCTION)
    {
        time = 0;
    }

    private bool BlocksAlrightToSpread(IntVector3 blockPos)
    {
        return terrain.GetBlock(blockPos) == RED_ROCK_ID &&
               terrain.GetBlock(blockPos + new IntVector3(0, 1, 0)) == AIR_ID;
    }
    
    protected override bool CanSpreadTo(IntVector3 blockPos)
    {
        return BlocksAlrightToSpread(blockPos) && 
            GAS_REQUIREMENTS.All(kvPair => atmosphere.GetGasProgress(kvPair.Key) >= kvPair.Value);
    }

    public override bool PlantOn(IntVector3 blockPos)
    {
        List<IntVector3> blockPosList = new List<IntVector3>()
        {
            blockPos
        };
        return PlantOn(blockPosList);
    }

    public void UpdateActive(IntVector3 blockPos)
    {
        if (plantBlocks.Contains(blockPos))
        {
            if (adjacentBlockVectors.Any(delta => BlocksAlrightToSpread(delta+blockPos)))
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
        foreach(IntVector3 delta in adjacentBlockVectors)
        {
            UpdateActive(blockPos + delta);
        }
    }

    public bool PlantOn(List<IntVector3> blockPosList)
    {
        List<IntVector3> validBlocks = (from blockPos in blockPosList
                                        where CanSpreadTo(blockPos)
                                        select blockPos).ToList();
        
        if (validBlocks.Count == 0)
            return false;

        Tuple<IntVector3, byte>[] blocksToChange = new Tuple<IntVector3, byte>[validBlocks.Count];

        int idx = 0;
        foreach (IntVector3 blockPos in validBlocks)
        {
            blocksToChange[idx++] = Tuple.Create(blockPos, GRASS_BLOCK_ID);
        }

        terrain.SetBlocks(blocksToChange);
        plantBlocks.UnionWith(validBlocks);
        foreach (IntVector3 blockPos in validBlocks)
        {
            // TODO: think performance, this probably checks same blocks multiple times
            RespondToChangedGrassiness(blockPos);
        }
        time = 0;
        return true;
    }

    public override void LifeCycle(float delta)
    {
        time += delta;
        if (time < LIFECYCLE_TICK_TIME || plantBlocks.Count == 0)
            return;
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
        List<IntVector3> blocksToChange = new List<IntVector3>();
        // with small probability, pick random point and spread to adjacent block
        for (double spreadNo = plantActiveBlocks.Count * spreadChance; spreadNo > 0; spreadNo--)
        {
            if (spreadNo < 1 && randGen.NextDouble() > spreadNo)
                return;

            // find a grass block that still exists
            IntVector3 block = plantActiveBlocks.ElementAt(randGen.Next(plantActiveBlocks.Count));

            // get adjacent blocks
            List<IntVector3> adjacentBlocks = new List<IntVector3>();
            foreach (IntVector3 v in adjacentBlockVectors)
            {
                if (CanSpreadTo(block + v))
                {
                    adjacentBlocks.Add(block + v);
                }
            }

            // choose an adjacent block and plant on it
            if (adjacentBlocks.Count > 0)
            {
                blocksToChange.Add(adjacentBlocks[randGen.Next(adjacentBlocks.Count)]);
            }
        }
        PlantOn(blocksToChange);
    }
}
