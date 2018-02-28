using Godot;
using System;

public class Chunk : Spatial
{
    public static IntVector3 CHUNK_SIZE = new IntVector3(16, 256, 16);
    public static float BLOCK_SIZE = 0.5f;

    private byte[,,] blocks;
    private MeshInstance meshInstance;
    private StaticBody body;
    private CollisionShape collider;

    private Terrain terrain;
    private IntVector2 chunkCoords;

    public byte GetBlockInChunk(IntVector3 position)
    {
        return GetBlockInChunk(position.x, position.y, position.z);
    }

    public byte GetBlockInChunk(int x, int y, int z)
    {
        if(x < 0 || x >= CHUNK_SIZE.x || y < 0 || y >= CHUNK_SIZE.y || z < 0 || z >= CHUNK_SIZE.z)
            return 0; //Maybe should throw exception/return null here ??
        else
            return blocks[x,y,z];
    }

    public void SetBlockInChunk(IntVector3 position, byte block)
    {
        SetBlockInChunk(position.x, position.y, position.z, block);
    }

    public void SetBlockInChunk(int x, int y, int z, byte block)
    {
        if(x < 0 || x >= CHUNK_SIZE.x || y < 0 || y >= CHUNK_SIZE.y || z < 0 || z >= CHUNK_SIZE.z)
            return; //Maybe should return false here?
        else
            blocks[x,y,z] = block;
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

    bool generationFinished = false;
    SurfaceTool surfaceTool = new SurfaceTool();
    SpatialMaterial material = new SpatialMaterial();

    public void UpdateMesh()
    {
        System.Threading.Thread thread = new System.Threading.Thread(() => 
        {
            GenerateSurface();
            generationFinished = true;
        });
        thread.Priority = System.Threading.ThreadPriority.Lowest;
        thread.Start();
    }

    private void GenerateSurface()
    {        
        surfaceTool.Begin(Mesh.PrimitiveType.Triangles);        

        for(int x = 0; x < CHUNK_SIZE.x; x++)
        {
            for(int y = 0; y < CHUNK_SIZE.y; y++)
            {
                for(int z = 0; z < CHUNK_SIZE.z; z++)
                {
                    byte blockType = blocks[x,y,z];
                    if(blockType == 0)
                        continue;

                    Block block = Game.GetBlock(blockType);

                    IntVector3 localIndex = new IntVector3(x, y, z);

                    //Index in world space
                    IntVector3 index = localIndex + new IntVector3(chunkCoords.x * CHUNK_SIZE.x, 0, chunkCoords.y * CHUNK_SIZE.z);

                    //Position in chunk
                    Vector3 localBlockPosition = localIndex * BLOCK_SIZE;
                    
                    if(terrain.GetBlock(index.x + 1, index.y, index.z) == 0)
                        block.AddPosXFace(ref surfaceTool, localBlockPosition, blockType);
                    if(terrain.GetBlock(index.x - 1, index.y, index.z) == 0)
                        block.AddNegXFace(ref surfaceTool, localBlockPosition, blockType);
                    if(terrain.GetBlock(index.x, index.y + 1, index.z) == 0)
                        block.AddPosYFace(ref surfaceTool, localBlockPosition, blockType);
                    if(terrain.GetBlock(index.x, index.y - 1, index.z) == 0 && y != 0) //Don't draw bottom face
                        block.AddNegYFace(ref surfaceTool, localBlockPosition, blockType);
                    if(terrain.GetBlock(index.x, index.y, index.z + 1) == 0)
                        block.AddPosZFace(ref surfaceTool, localBlockPosition, blockType);
                    if(terrain.GetBlock(index.x, index.y, index.z - 1) == 0)
                        block.AddNegZFace(ref surfaceTool, localBlockPosition, blockType);
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
        this.Translate(new IntVector3((int) (coords.x * CHUNK_SIZE.x * BLOCK_SIZE), 0, (int) (coords.y * CHUNK_SIZE.z * BLOCK_SIZE)));
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