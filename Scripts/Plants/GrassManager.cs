using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class GrassManager : PlantManager
{
    private static byte grassBlock = Game.GetBlockId<GrassBlock>();
    private static byte redRock = Game.GetBlockId<RedRock>();

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
        [Gas.CARBON_DIOXIDE] = 0.6f,
    };

    private static IntVector3[] adjacentBlockVectors = new IntVector3[12] {
        new IntVector3(-1, -1, 0), new IntVector3(-1, 0, 0), new IntVector3(-1, 1, 0),
        new IntVector3(0, -1, -1), new IntVector3(0, 0, -1), new IntVector3(0, 1, -1),
        new IntVector3(0, -1, 1), new IntVector3(0, 0, 1), new IntVector3(0, 1, 1),
        new IntVector3(1, -1, 0), new IntVector3(1, 0, 0), new IntVector3(1, 1, 0)
    };

    private float time;

    private Plants plants;
    private Dictionary<IntVector3, PhysicsBody> physicsBodies;

    public GrassManager(Plants plants) : base(plants)
    {
        this.plants = plants;

        SPREAD_CHANCE = 0.501;
        time = 0;

        GAS_DELTAS = GAS_PRODUCTION;


        physicsBodies = new Dictionary<IntVector3, PhysicsBody>();
    }

    protected override bool Valid(IntVector3 blockPos)
    {
        return terrain.GetBlock(blockPos) == redRock &&
               terrain.GetBlock(blockPos + new IntVector3(0, 1, 0)) == 0 &&
               atmosphere.GetGasAmt(Gas.NITROGEN) > 0.01 &&
               atmosphere.GetGasAmt(Gas.CARBON_DIOXIDE) > 0.01;
    }

    public override bool PlantOn(IntVector3 blockPos)
    {
        List<IntVector3> blockPosList = new List<IntVector3>()
        {
            blockPos
        };
        return PlantOn(blockPosList);
    }

    public bool PlantOn(List<IntVector3> blockPosList)
    {
        List<IntVector3> validBlocks = (from blockPos in blockPosList
                                        where Valid(blockPos)
                                        select blockPos).ToList<IntVector3>();

        if (validBlocks.Count == 0)
            return false;

        Tuple<IntVector3, byte>[] blocksToChange = new Tuple<IntVector3, byte>[validBlocks.Count];

        int idx = 0;
        foreach (IntVector3 blockPos in blockPosList)
        {
            blocksToChange[idx++] = Tuple.Create(blockPos, grassBlock);

            PhysicsBody physicsBody = new KinematicBody();
            physicsBody.SetTranslation(blockPos);

            CollisionShape collisionShape = new CollisionShape();
            BoxShape b = new BoxShape();
            b.SetExtents(new Vector3(Block.SIZE, Block.SIZE, Block.SIZE) / 2);
            collisionShape.SetShape(b);

            physicsBody.AddChild(collisionShape);

            physicsBodies[blockPos] = physicsBody;
            plants.AddChild(physicsBody);
        }

        terrain.SetBlocks(blocksToChange);
        blocks.UnionWith(validBlocks);
        time = 0;
        return true;
    }

    public override void LifeCycle(float delta)
    {
        time += delta;
        if (time < LIFECYCLE_TICK_TIME || blocks.Count == 0)
            return;
        time = 0;

        // remove blocks that are no longer grass
        List<IntVector3> blocksToRemove = (from block in blocks
                                           where !physicsBodies[block].IsInGroup("alive")
                                           select block).ToList<IntVector3>();

        blocks.ExceptWith(blocksToRemove);

        List<Tuple<IntVector3, byte>> blocksToChange = new List<Tuple<IntVector3, byte>>();
        foreach (IntVector3 block in blocksToRemove)
            blocksToChange.Add(Tuple.create(block, redRock));

        // kill off some grass if there is too little gas
        float numberToDie = GAS_REQUIREMENTS.Sum(kvPair => 5 * Mathf.Max(kvPair.Value - atmosphere.GetGasAmt(kvPair.Key), 0));

        while (numberToDie > 0)
        {
            if (blocks.Count == 0)
                break;

            if (numberToDie < 1 && randGen.NextDouble() > numberToDie)
                break;

            int idx = randGen.Next(blocks.Count);
            IntVector3 block = blocks.ElementAt(idx);
            physicsBodies[block].RemoveFromGroup("alive");
            physicsBodies[block].QueueFree();
            physicsBodies.Remove(block);
            blocks.Remove(block);

            blocksToChange.Add(Tuple.create(block, redRock));
            numberToDie--;
        }
        terrain.SetBlocks(blocksToChange);

        Spread();

    }

    protected override void Spread()
    {
        List<IntVector3> blocksToChange = new List<IntVector3>();
        // with small probability, pick random point and spread to adjacent block
        for (double spreadNo = blocks.Count * SPREAD_CHANCE; spreadNo > 0; spreadNo--)
        {
            if (spreadNo < 1 && randGen.NextDouble() > spreadNo)
                return;

            // find a grass block that still exists
            int idx = randGen.Next(blocks.Count);
            IntVector3 block = blocks.ElementAt(idx);

            // get adjacent blocks
            List<IntVector3> adjacentBlocks = new List<IntVector3>();
            foreach (IntVector3 v in adjacentBlockVectors)
            {
                if (Valid(block + v))
                    adjacentBlocks.Add(block + v);
            }

            // choose an adjacent block and plant on it
            if (adjacentBlocks.Count > 0)
                blocksToChange.Add(adjacentBlocks[randGen.Next(adjacentBlocks.Count)]);
        }
        PlantOn(blocksToChange);
    }
}
