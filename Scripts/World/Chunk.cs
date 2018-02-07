using Godot;
using System;

public class Chunk : Spatial
{
    public static IntVector3 CHUNK_SIZE = new IntVector3(16, 256, 16);
    public static float BLOCK_SIZE = 0.5f;

    private byte[,,] blocks;
    private SurfaceTool surfaceTool = new SurfaceTool();
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

    public void UpdateMesh()
    {
        ArrayMesh mesh = new ArrayMesh();
        SpatialMaterial material = new SpatialMaterial();
        material.AlbedoTexture = Game.TextureAtlas;
        material.SetDiffuseMode(SpatialMaterial.DiffuseMode.Lambert);
        material.SetSpecularMode(SpatialMaterial.SpecularMode.Disabled);
        material.SetMetallic(0);
        material.SetRoughness(1);

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

                    IntVector3 blockPos = new IntVector3(x, y, z);

                    Vector3 worldBlockPos = blockPos + new Vector3(chunkCoords.x * CHUNK_SIZE.x, 0, chunkCoords.y * CHUNK_SIZE.z);

                    int sx = (int) worldBlockPos.x;
                    int sy = (int) worldBlockPos.y;
                    int sz = (int) worldBlockPos.z;

                    Vector3 graphicalBlockPos = blockPos * BLOCK_SIZE;
                    
                    if(terrain.GetBlock(sx+1,sy,sz) == 0)
                        block.AddPosXFace(ref surfaceTool, graphicalBlockPos, blockType);
                    if(terrain.GetBlock(sx-1,sy,sz) == 0)
                        block.AddNegXFace(ref surfaceTool, graphicalBlockPos, blockType);
                    if(terrain.GetBlock(sx,sy+1,sz) == 0)
                        block.AddPosYFace(ref surfaceTool, graphicalBlockPos, blockType);
                    if(terrain.GetBlock(sx,sy-1,sz) == 0 && y != 0)
                        block.AddNegYFace(ref surfaceTool, graphicalBlockPos, blockType);
                    if(terrain.GetBlock(sx,sy,sz+1) == 0)
                        block.AddPosZFace(ref surfaceTool, graphicalBlockPos, blockType);
                    if(terrain.GetBlock(sx,sy,sz-1) == 0)
                        block.AddNegZFace(ref surfaceTool, graphicalBlockPos, blockType);
                }
            }
        }

        surfaceTool.GenerateNormals();

        surfaceTool.SetMaterial(material);

        surfaceTool.Index();

        mesh = surfaceTool.Commit();

        collider.Shape = mesh.CreateTrimeshShape();

        meshInstance.SetMesh(mesh);
    }

    //Just generate terrain data
    public void SoftLoad()
    {
        blocks = terrain.WorldGenerator.GetChunk(chunkCoords.x, chunkCoords.y, CHUNK_SIZE.x, CHUNK_SIZE.y, CHUNK_SIZE.z);
    }

    //Generate graphical data aswell
    public void HardLoad()
    {
        meshInstance = new MeshInstance();
        this.AddChild(meshInstance);

        body = new StaticBody();
        this.AddChild(body);
        collider = new CollisionShape();
        body.AddChild(collider);
        
        UpdateMesh();
    }

    public Chunk(Terrain terrain, IntVector2 coords)
    {
        this.terrain = terrain;
        this.Translate(new IntVector3((int) (coords.x * CHUNK_SIZE.x * BLOCK_SIZE), 0, (int) (coords.y * CHUNK_SIZE.z * BLOCK_SIZE)));

        this.chunkCoords = coords;
    }
}