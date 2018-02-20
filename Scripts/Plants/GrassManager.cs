using System.Collections.Generic;

public class GrassManager : PlantManager
{
    private static byte grassBlock = Game.GetBlockId<GrassBlock>();
    private static byte redRock = Game.GetBlockId<RedRock>();

    private static IntVector3[] adjacentBlockVectors = new IntVector3[12] {
        new IntVector3(-1, -1, 0), new IntVector3(-1, 0, 0), new IntVector3(-1, 1, 0),
        new IntVector3(0, -1, -1), new IntVector3(0, 0, -1), new IntVector3(0, 1, -1),
        new IntVector3(0, -1, 1), new IntVector3(0, 0, 1), new IntVector3(0, 1, 1),
        new IntVector3(1, -1, 0), new IntVector3(1, 0, 0), new IntVector3(1, 1, 0)
    };

    private float time;

    public GrassManager(Terrain terrain) : base(terrain) {
        SPREAD_CHANCE = 0.01;
        time = 0;
    }

    private bool Valid(IntVector3 blockPos)
    {
        return terrain.GetBlock(blockPos) == redRock &&
               terrain.GetBlock(blockPos + new IntVector3(0, 1, 0)) == 0;
    }

    public override bool PlantOn(IntVector3 blockPos)
    {
        if (!Valid(blockPos))
            return false;

        terrain.SetBlock(blockPos, grassBlock);
        blocks.Add(blockPos);
        time = 0;
        return true;
    }

    public override void Spread(float delta)
    {
        time += delta;
        if (time < SPREAD_TIME || blocks.Count == 0)
            return;

        time = 0;

        // with small probability, pick random point and spread to adjacent block

        for (double spread_no = blocks.Count*SPREAD_CHANCE; spread_no > 0; spread_no--)
        {
            if (spread_no < 1 && randGen.NextDouble() > spread_no)
                return;

            // find a grass block that still exists
            int idx = randGen.Next(blocks.Count);
            IntVector3 block = blocks[idx];
            while (terrain.GetBlock(block) == 0)
            {
                blocks.RemoveAt(idx);

                if (blocks.Count == 0)
                    return;

                idx = randGen.Next(blocks.Count);
                block = blocks[idx];
            }

            // get adjacent blocks
            List<IntVector3> adjacentBlocks = new List<IntVector3>();
            foreach (IntVector3 v in adjacentBlockVectors)
            {
                if (Valid(block + v))
                    adjacentBlocks.Add(block + v);
            }

            // choose an adjacent block and plant on it
            if (adjacentBlocks.Count > 0)
            {
                PlantOn(adjacentBlocks[randGen.Next(adjacentBlocks.Count)]);
                return;
            }
        }

        return;
    }
}
