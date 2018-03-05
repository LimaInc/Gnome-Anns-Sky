using Godot;
using System;
using ST = System.Threading;

public class Chunk : Spatial
{
    private const byte AIR_ID = WorldGenerator.AIR_ID;

    public static IntVector3 SIZE = new IntVector3(16, 256, 16);

    private byte[,,] blocks;
    private MeshInstance meshInstance;
    private StaticBody body;
    private CollisionShape collider;

    private Terrain terrain;
    private IntVector2 chunkCoords;

    private bool generationFinished = false;
    private SurfaceTool surfaceTool = new SurfaceTool();
    private SpatialMaterial material = new SpatialMaterial();

    private ST.Thread generationThread;

    public byte GetBlockInChunk(IntVector3 position)
    {
        return GetBlockInChunk(position.x, position.y, position.z);
    }

    public byte GetBlockInChunk(int x, int y, int z)
    {
        if (x < 0 || x >= SIZE.x || y < 0 || y >= SIZE.y || z < 0 || z >= SIZE.z)
        {
            throw new ArgumentOutOfRangeException("Tried to get block " + new IntVector3(x, y, z)+
                ", but chunk size is "+SIZE+
                "; Debug message: "+Debug.Message);
        }
        else
        {
            return blocks[x, y, z];
        }
    }

    public byte? TrySetBlockInChunk(IntVector3 position, byte block)
    {
        return TrySetBlockInChunk(position.x, position.y, position.z, block);
    }

    public byte? TrySetBlockInChunk(int x, int y, int z, byte block)
    {
        if (IsPosInChunk(x,y,z))
        {
            byte prevBlock = blocks[x, y, z];
            blocks[x, y, z] = block;
            return prevBlock;
        }
        else
        {
            return null;
        }
    }

    public bool IsPosInChunk(IntVector3 pos)
    {
        return IsPosInChunk(pos.x, pos.y, pos.z);
    }

    public bool IsPosInChunk(int x, int y, int z)
    {
        return x >= 0 && x < SIZE.x && y >= 0 && y < SIZE.y && z >= 0 && z < SIZE.z;
    }

    public override void _Process(float delta)
    {
        if(generationFinished)
        {
            generationFinished = false;

            ArrayMesh mesh = new ArrayMesh();

            mesh = surfaceTool.Commit();

            collider.Shape = mesh.CreateTrimeshShape();

            meshInstance.SetMesh(mesh);
        }
    }

    public void UpdateMesh()
    {
        if(generationThread != null && generationThread.ThreadState == ST.ThreadState.Running)
            generationThread.Abort();
        
        generationThread = new ST.Thread(() =>
        {
            GenerateSurface();
            generationFinished = true;
        })
        {
            Priority = ST.ThreadPriority.Highest
        };
        
        generationThread.Start();
    }

    private void GenerateSurface()
    {        
        surfaceTool.Begin(Mesh.PrimitiveType.Triangles);        

        for(int x = 0; x < SIZE.x; x++)
        {
            for(int y = 0; y < SIZE.y; y++)
            {
                for(int z = 0; z < SIZE.z; z++)
                {
                    byte blockType = blocks[x,y,z];
                    if(blockType == 0)
                        continue;

                    Block block = Game.GetBlock(blockType);

                    IntVector3 localIndex = new IntVector3(x, y, z);

                    //Index in world space
                    IntVector3 index = localIndex + new IntVector3(chunkCoords.x * SIZE.x, 0, chunkCoords.y * SIZE.z);

                    //Position in chunk
                    Vector3 localBlockPosition = localIndex * Block.SIZE;
                    
                    if (terrain.GetBlock(index.x + 1, index.y, index.z) == AIR_ID)
                    {
                        block.AddPosXFace(ref surfaceTool, localBlockPosition, blockType);
                    }
                    if (terrain.GetBlock(index.x - 1, index.y, index.z) == AIR_ID)
                    {
                        block.AddNegXFace(ref surfaceTool, localBlockPosition, blockType);
                    }
                    if (terrain.GetBlock(index.x, index.y + 1, index.z) == AIR_ID)
                    {
                        block.AddPosYFace(ref surfaceTool, localBlockPosition, blockType);
                    }
                    if (index.y != 0 && terrain.GetBlock(index.x, index.y - 1, index.z) == AIR_ID) //Don't draw bottom face
                    {
                        block.AddNegYFace(ref surfaceTool, localBlockPosition, blockType);
                    }
                    if (terrain.GetBlock(index.x, index.y, index.z + 1) == AIR_ID)
                    {
                        block.AddPosZFace(ref surfaceTool, localBlockPosition, blockType);
                    }
                    if (terrain.GetBlock(index.x, index.y, index.z - 1) == AIR_ID)
                    {
                        block.AddNegZFace(ref surfaceTool, localBlockPosition, blockType);
                    }
                }
            }
        }

        surfaceTool.GenerateNormals();

        surfaceTool.SetMaterial(material);

        surfaceTool.Index();
    }

    public Chunk(Terrain terrain, IntVector2 coords, byte[,,] blocks)
    {
        this.terrain = terrain;
        this.Translate(new IntVector3((int) (coords.x * SIZE.x * Block.SIZE), 0, (int) (coords.y * SIZE.z * Block.SIZE)));
        this.blocks = blocks;

        this.chunkCoords = coords;

        meshInstance = new MeshInstance();
        this.AddChild(meshInstance);

        body = new StaticBody();
        this.AddChild(body);
        collider = new CollisionShape();
        body.AddChild(collider);

        material.AlbedoTexture = Game.TextureAtlas;
        material.SetDiffuseMode(SpatialMaterial.DiffuseMode.Lambert);
        material.SetSpecularMode(SpatialMaterial.SpecularMode.Disabled);
        material.SetMetallic(0);
        material.SetRoughness(1);

    }
}