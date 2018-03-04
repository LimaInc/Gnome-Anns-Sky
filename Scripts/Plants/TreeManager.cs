using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class TreeManager : PlantManager
{
    private static byte grassBlock = Game.GetBlockId<GrassBlock>();
    private static byte leafBlock = Game.GetBlockId<LeafBlock>();
    private static byte redRock = Game.GetBlockId<RedRock>();
    private static byte treeBlock = Game.GetBlockId<TreeBlock>();
    private static byte airBlock = WorldGenerator.AIR_ID;

    private static Tuple<IntVector3, byte>[] treeBlockVectors = new Tuple<IntVector3, byte>[48];

    private static int GRID_SIZE = 10;
    private static double MIN_DISTANCE = 10*Math.Sqrt(2);
    private static double MIN_VERTICAL_DISTANCE = 5;

    private Dictionary<Tuple<int, int>, List<IntVector3>> grid;

    private float time;
    private Dictionary<IntVector3, PhysicsBody> physicsBodies;
    private Plants plants;

    public TreeManager(Plants plants) : base(plants)
    {
        this.plants = plants;

        GAS_DELTAS = new Dictionary<Gas, float>
        {
            [Gas.OXYGEN] = 0.00005f,
            [Gas.NITROGEN] = -0.00005f,
            [Gas.CARBON_DIOXIDE] = -0.00005f
        };

        SPREAD_CHANCE = 0.01;
        time = 0;
        grid = new Dictionary<Tuple<int, int>, List<IntVector3>>();

        physicsBodies = new Dictionary<IntVector3, PhysicsBody>();

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

    protected override bool Valid(IntVector3 blockPos)
    {
        if (terrain.GetBlock(blockPos) != redRock &&
                terrain.GetBlock(blockPos) != grassBlock &&
                atmosphere.GetGasAmt(Gas.NITROGEN) < 0.01 &&
                atmosphere.GetGasAmt(Gas.CARBON_DIOXIDE) < 0.01)
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

        CollisionShape collisionShape = new CollisionShape();
        BoxShape b = new BoxShape();
        b.SetExtents(new Vector3(Block.SIZE*3.0f, 4.0f*Block.SIZE, Block.SIZE*3.0f)); //trees are 8ish blocks tall
        collisionShape.SetShape(b);

        PhysicsBody physicsBody = new StaticBody();
        physicsBody.AddToGroup("plants");
        physicsBody.AddToGroup("alive");

        
        //Don't collide with player
        physicsBody.SetCollisionLayerBit(0, false);
        physicsBody.SetCollisionMaskBit(0, false);

        //But still collide with giants
        physicsBody.SetCollisionLayerBit(2, true);

        //And still get picked up by the giant areas
        physicsBody.SetCollisionMaskBit(31, true);

        Vector3 position = (blockPos + new Vector3(0, 5, 0)) * Block.SIZE + new Vector3(0,5,0); //the plus 5 to make it actually seen
        physicsBody.SetTranslation(position);
        GD.Print("Player position: ", ((PhysicsBody)plants.GetTree().GetRoot().GetNode("Game").GetNode("Player")).GetTranslation().ToString());
        GD.Print("Planted position: ", position);

        physicsBody.AddChild(collisionShape);

        physicsBodies[blockPos] = physicsBody;
        plants.AddChild(physicsBody);

        return true;
    }

    public override void LifeCycle(float delta)
    {
        time += delta;
        if (time < LIFECYCLE_TICK_TIME || blocks.Count == 0)
            return;

        time = 0;

        List<IntVector3> blocksToRemove = (from block in blocks
                                           where !physicsBodies[block].IsInGroup("alive")
                                           select block).ToList();

        blocks.ExceptWith(blocksToRemove);

        List<Tuple<IntVector3, byte>> blocksToChange = new List<Tuple<IntVector3, byte>>();
        foreach (IntVector3 position in blocksToRemove)
        {
            for (int i = 0; i < 48; i++)
            {
                blocksToChange.Add(Tuple.Create(position + treeBlockVectors[i].Item1, airBlock));
            }
        }

        terrain.SetBlocks(blocksToChange);

        // in a full game, this would manage the tree growing from a sapling and then dying after some time
        Spread();
    }

    protected override void Spread()
    {
        // Bridson’s algorithm for Poisson-disc sampling
        // https://bost.ocks.org/mike/algorithms/
        for (double spreadNo = blocks.Count*SPREAD_CHANCE; spreadNo > 0; spreadNo--)
        {
            if (spreadNo < 1 && randGen.NextDouble() > spreadNo)
                return;

            // find a tree that still exists
            int idx = randGen.Next(blocks.Count);
            IntVector3 block = blocks.ElementAt(idx);
            while (terrain.GetBlock(block + new IntVector3(0, 1, 0)) != treeBlock) // test for tree block above position
            {
                blocks.Remove(block);
                grid[Tuple.Create(block.x/GRID_SIZE, block.z/GRID_SIZE)].Remove(block);

                if (blocks.Count == 0)
                    return;

                idx = randGen.Next(blocks.Count);
                block = blocks.ElementAt(idx);
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
