using System;
using System.Collections.Generic;

public class TreeManager : PlantManager
{
    private static byte grassBlock = Game.GetBlockId<GrassBlock>();
    private static byte leafBlock = Game.GetBlockId<LeafBlock>();
    private static byte redRock = Game.GetBlockId<RedRock>();
    private static byte treeBlock = Game.GetBlockId<TreeBlock>();

    private static Tuple<IntVector3, byte>[] treeBlockVectors = new Tuple<IntVector3, byte>[48];

    private static int GRID_SIZE = 10;
    private static double MIN_DISTANCE = 10*Math.Sqrt(2);
    private static double MIN_VERTICAL_DISTANCE = 5;

    private Dictionary<Tuple<int, int>, List<IntVector3>> grid;

    private float time;

    public TreeManager(Terrain terrain) : base(terrain)
    {
        SPREAD_CHANCE = 0.001;
        time = 0;
        grid = new Dictionary<Tuple<int, int>, List<IntVector3>>();

        int idx = 0;
        for (int y = 1; y < 6; y++)
        {
            treeBlockVectors[idx++] = Tuple.Create(new IntVector3(0, y, 0), treeBlock);
        }

        for (int x = -1; x < 2; x++)
        {
            for (int z = -1; z < 2; z++)
            {
                if (x != 0 || z != 0)
                    treeBlockVectors[idx++] = Tuple.Create(new IntVector3(x, 5, z), leafBlock);

                treeBlockVectors[idx++] = Tuple.Create(new IntVector3(x, 7, z), leafBlock);
            }
        }

        for (int x = -2; x < 3; x++)
        {
            for (int z = -2; z < 3; z++)
                treeBlockVectors[idx++] = Tuple.Create(new IntVector3(x, 6, z), leafBlock);
        }

        treeBlockVectors[idx++] = Tuple.Create(new IntVector3(0, 8, 0), leafBlock);
    }

    public bool Valid(IntVector3 blockPos)
    {
        if (terrain.GetBlock(blockPos) != redRock && terrain.GetBlock(blockPos) != grassBlock)
            return false;

        for (int i = 0; i < 48; i++)
        {
            if (terrain.GetBlock(blockPos + treeBlockVectors[i].Item1) != 0)
                return false;
        }
        // TODO: detect collision
        return true;
    }

    public override bool PlantOn(IntVector3 blockPos)
    {
        Tuple<IntVector3, byte>[] treeBlocks = new Tuple<IntVector3, byte>[48];
        for (int i = 0; i < 48; i++)
            treeBlocks[i] = Tuple.Create(blockPos + treeBlockVectors[i].Item1, treeBlockVectors[i].Item2);

        if (!Valid(blockPos))
            return false;

        terrain.SetBlocks(treeBlocks);
        blocks.Add(blockPos);
        Tuple<int, int> gridPos = Tuple.Create(blockPos.x/GRID_SIZE, blockPos.z/GRID_SIZE);
        if (!grid.ContainsKey(gridPos))
            grid[gridPos] = new List<IntVector3>();
        grid[gridPos].Add(blockPos);

        return true;
    }

    public override void Spread(float delta)
    {
        time += delta;
        if (time < SPREAD_TIME || blocks.Count == 0)
            return;

        time = 0;

        // Bridson’s algorithm for Poisson-disc sampling
        // https://bost.ocks.org/mike/algorithms/
        for (double spread_no = blocks.Count*SPREAD_CHANCE; spread_no > 0; spread_no--)
        {
            if (spread_no < 1 && randGen.NextDouble() > spread_no)
                return;

            // find a tree that still exists
            int idx = randGen.Next(blocks.Count);
            IntVector3 block = blocks[idx];
            while (terrain.GetBlock(block + new IntVector3(0, 1, 0)) == 0) // test for tree block above position
            {
                blocks.RemoveAt(idx);
                grid[Tuple.Create(block.x/GRID_SIZE, block.z/GRID_SIZE)].Remove(block);

                if (blocks.Count == 0)
                    return;

                idx = randGen.Next(blocks.Count);
                block = blocks[idx];
            }

            // generate a candidate tree
            double candidateAngle = randGen.NextDouble() * 2 * Math.PI;
            double candidateDistance = MIN_DISTANCE * (1 + randGen.NextDouble());
            IntVector3 candidate = block + new IntVector3((int) (candidateDistance * Math.Sin(candidateAngle)), 0,
                                                          (int) (candidateDistance * Math.Cos(candidateAngle)));

            bool candidateExists = false;
            for (int i = 0; i < MIN_DISTANCE / 2; i++)
            {
                if (Valid(candidate + new IntVector3(0, i, 0)))
                {
                    candidate += new IntVector3(0, i, 0);
                    candidateExists = true;
                    break;
                }

                if (i != 0 && Valid(candidate + new IntVector3(0, -i, 0)))
                {
                    candidate += new IntVector3(0, -i, 0);
                    candidateExists = true;
                    break;
                }
            }
            if (!candidateExists)
                continue;

            // test if other trees are near it
            Tuple<int, int> gridPos = Tuple.Create(candidate.x/GRID_SIZE, candidate.z/GRID_SIZE);

            bool candidateValid = true;
            for (int x = -2; x < 3; x++)
            {
                for (int z = -2; z < 3; z++)
                {
                    Tuple<int, int> testGridPos = Tuple.Create(gridPos.Item1 + x, gridPos.Item2 + z);

                    if (!grid.ContainsKey(testGridPos) || grid[testGridPos].Count == 0)
                        continue;

                    List<IntVector3> testTrees = grid[testGridPos];

                    for (int i = testTrees.Count - 1; i >= 0; i--)
                    {
                        IntVector3 testTree = testTrees[i];
                        if (terrain.GetBlock(testTree + new IntVector3(0, 1, 0)) == 0) // test for tree block above position
                        {
                            blocks.Remove(block);
                            testTrees.RemoveAt(i);
                            continue;
                        }

                        if (Math.Abs(testTree.y - candidate.y) < MIN_VERTICAL_DISTANCE &&
                                Math.Sqrt(Math.Pow(testTree.x - candidate.x, 2) + Math.Pow(testTree.z - candidate.z, 2)) < MIN_DISTANCE)
                        {
                            candidateValid = false;
                            break;
                        }
                    }
                }

                if (!candidateValid)
                    break;
            }

            if (candidateValid)
            {
                PlantOn(candidate);
                return;
            }
        }

        return;
    }
}