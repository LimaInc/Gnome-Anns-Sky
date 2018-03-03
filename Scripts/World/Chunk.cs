using Godot;
using System;

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

    bool generationFinished = false;
    SurfaceTool surfaceTool = new SurfaceTool();
    SpatialMaterial material = new SpatialMaterial();

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

    public bool TrySetBlockInChunk(IntVector3 position, byte block)
    {
        return TrySetBlockInChunk(position.x, position.y, position.z, block);
    }

    public bool TrySetBlockInChunk(int x, int y, int z, byte block)
    {
        if (x < 0 || x >= SIZE.x || y < 0 || y >= SIZE.y || z < 0 || z >= SIZE.z)
        {
            return false;
        }
        else
        {
            blocks[x, y, z] = block;
            return true;
        }
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
        System.Threading.Thread thread = new System.Threading.Thread(() =>
        {
            GenerateSurface();
            generationFinished = true;
        })
        {
            Priority = System.Threading.ThreadPriority.Lowest
        };
        thread.Start();
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