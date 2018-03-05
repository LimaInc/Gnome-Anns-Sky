using Godot;
using System;
using System.Collections.Generic;

public class WheatManager : PlantManager
{
    private static byte wheatBlock = Game.GetBlockId<WheatBlock>();
    private static byte grassBlock = Game.GetBlockId<GrassBlock>();
    private static byte redRock = Game.GetBlockId<RedRock>();

    private const float BASE_GAS_PRODUCTION = 0.00000001f;
    public static readonly IDictionary<Gas, float> GAS_PRODUCTION = new Dictionary<Gas, float>
    {
        [Gas.OXYGEN] = BASE_GAS_PRODUCTION,
        [Gas.NITROGEN] = -BASE_GAS_PRODUCTION,
        [Gas.CARBON_DIOXIDE] = -BASE_GAS_PRODUCTION,
    };

    private float time;
    private float GROWING_TIME = 10.0f;

    private Queue<Tuple<IntVector3, float>> growingQueue;

    public WheatManager(Plants plants_) : base(plants_, 0.0, GAS_PRODUCTION)
    {
        growingQueue = new Queue<Tuple<IntVector3, float>>();
        time = 0;
        return;
    }

    protected override bool CanGrowOn(IntVector3 blockPos)
    {
        return (terrain.GetBlock(blockPos) == redRock || terrain.GetBlock(blockPos) == grassBlock) &&
               terrain.GetBlock(blockPos + new IntVector3(0, 1, 0)) == 0 &&
               atmosphere.GetGasAmt(Gas.NITROGEN) > 0.01 &&
               atmosphere.GetGasAmt(Gas.CARBON_DIOXIDE) > 0.1;
    }

    public override bool PlantOn(IntVector3 blockPos)
    {
        if (!CanGrowOn(blockPos))
            return false;

        IntVector3 wheatBlockPos = blockPos + new IntVector3(0, 1, 0);
        terrain.SetBlock(wheatBlockPos, wheatBlock);
        // blocks.Add(wheatBlockPos);

        growingQueue.Enqueue(Tuple.Create(wheatBlockPos, time + GROWING_TIME));

        return true;
    }

    public override void LifeCycle(float delta)
    {
        time += delta;

        Tuple<IntVector3, float> block;
        while (growingQueue.Count > 0)
        {
            block = growingQueue.Peek();
            if (block?.Item2 > time)
                break;

            growingQueue.Dequeue();

            // TODO: group all into a batch grow
            if (Grow(block.Item1))
            {
                growingQueue.Enqueue(Tuple.Create(block.Item1 + new IntVector3(0, 1, 0), time + GROWING_TIME));
            }
        }
    }

    public bool Grow(IntVector3 blockPos)
    {
        // check for space
        IntVector3 newBlockPos = blockPos + new IntVector3(0, 1, 0);
        if (terrain.GetBlock(newBlockPos) != 0)
            return false;

        // check it is growing on other wheat and then on soil
        int i = 0;
        for (; i < 3; i++)
        {
            if (terrain.GetBlock(blockPos - new IntVector3(0, i, 0)) != wheatBlock)
                break;
        }
        if (terrain.GetBlock(blockPos - new IntVector3(0, i, 0)) != redRock)
            return false;

        terrain.SetBlock(newBlockPos, wheatBlock);

        return true;
    }

    protected override void Spread()
    {
        return;
    }

    public override void HandleBlockChange(byte oldId, byte newId, IntVector3 pos)
    {
        // TODO: check if the wheat is still there, maybe?
    }
}
