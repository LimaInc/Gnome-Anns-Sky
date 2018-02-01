using Godot;
using System;

public class Chunk : Spatial
{
    public static IntVector3 CHUNK_SIZE = new IntVector3(16, 256, 16);
    public static float BLOCK_SIZE = 0.5f;

    private byte[,,] blocks;
    private SurfaceTool surfaceTool = new SurfaceTool();
    private WorldGenerator worldGenerator;
    private Texture textureAtlas;
    private Rect2[] textureUVs;

    private MeshInstance meshInstance;
    private StaticBody body;
    private CollisionShape collider;

    private Terrain terrain;
    private IntVector2 chunkCoords;

    private void AddPosXFace(Vector3 origin, byte blockType)
    {
        Rect2 uvs = textureUVs[blockType-1];

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + BLOCK_SIZE * new Vector3(+0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + BLOCK_SIZE * new Vector3(+0.5f, +0.5f, +0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(uvs.Size.x, 0));
        surfaceTool.AddVertex(origin + BLOCK_SIZE * new Vector3(+0.5f, +0.5f, -0.5f));

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + BLOCK_SIZE * new Vector3(+0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(0, uvs.Size.y));
        surfaceTool.AddVertex(origin + BLOCK_SIZE * new Vector3(+0.5f, -0.5f, +0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + BLOCK_SIZE * new Vector3(+0.5f, +0.5f, +0.5f));
    }
    private void AddNegXFace(Vector3 origin, byte blockType)
    {
        Rect2 uvs = textureUVs[blockType-1];

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + BLOCK_SIZE * new Vector3(-0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(uvs.Size.x, 0));
        surfaceTool.AddVertex(origin + BLOCK_SIZE * new Vector3(-0.5f, +0.5f, -0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + BLOCK_SIZE * new Vector3(-0.5f, +0.5f, +0.5f));

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + BLOCK_SIZE * new Vector3(-0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + BLOCK_SIZE * new Vector3(-0.5f, +0.5f, +0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(0, uvs.Size.y));
        surfaceTool.AddVertex(origin + BLOCK_SIZE * new Vector3(-0.5f, -0.5f, +0.5f));
    }
    private void AddPosYFace(Vector3 origin, byte blockType)
    {
        Rect2 uvs = textureUVs[blockType-1];

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + BLOCK_SIZE * new Vector3(-0.5f, +0.5f, -0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(uvs.Size.x, 0));
        surfaceTool.AddVertex(origin + BLOCK_SIZE * new Vector3(+0.5f, +0.5f, -0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + BLOCK_SIZE * new Vector3(+0.5f, +0.5f, +0.5f));

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + BLOCK_SIZE * new Vector3(-0.5f, +0.5f, -0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + BLOCK_SIZE * new Vector3(+0.5f, +0.5f, +0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(0, uvs.Size.y));
        surfaceTool.AddVertex(origin + BLOCK_SIZE * new Vector3(-0.5f, +0.5f, +0.5f));
    }
    private void AddNegYFace(Vector3 origin, byte blockType)
    {
        Rect2 uvs = textureUVs[blockType-1];

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + BLOCK_SIZE * new Vector3(-0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + BLOCK_SIZE * new Vector3(+0.5f, -0.5f, +0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(uvs.Size.x, 0));
        surfaceTool.AddVertex(origin + BLOCK_SIZE * new Vector3(+0.5f, -0.5f, -0.5f));

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + BLOCK_SIZE * new Vector3(-0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(0, uvs.Size.y));
        surfaceTool.AddVertex(origin + BLOCK_SIZE * new Vector3(-0.5f, -0.5f, +0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + BLOCK_SIZE * new Vector3(+0.5f, -0.5f, +0.5f));
    }
    private void AddPosZFace(Vector3 origin, byte blockType)
    {
        Rect2 uvs = textureUVs[blockType-1];

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + BLOCK_SIZE * new Vector3(-0.5f, -0.5f, +0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + BLOCK_SIZE * new Vector3(+0.5f, +0.5f, +0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(uvs.Size.x, 0));
        surfaceTool.AddVertex(origin + BLOCK_SIZE * new Vector3(+0.5f, -0.5f, +0.5f));

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + BLOCK_SIZE * new Vector3(-0.5f, -0.5f, +0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(0, uvs.Size.y));
        surfaceTool.AddVertex(origin + BLOCK_SIZE * new Vector3(-0.5f, +0.5f, +0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + BLOCK_SIZE * new Vector3(+0.5f, +0.5f, +0.5f));
    }
    private void AddNegZFace(Vector3 origin, byte blockType)
    {
        Rect2 uvs = textureUVs[blockType-1];

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + BLOCK_SIZE * new Vector3(-0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(uvs.Size.x, 0));
        surfaceTool.AddVertex(origin + BLOCK_SIZE * new Vector3(+0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + BLOCK_SIZE * new Vector3(+0.5f, +0.5f, -0.5f));

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + BLOCK_SIZE * new Vector3(-0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + BLOCK_SIZE * new Vector3(+0.5f, +0.5f, -0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(0, uvs.Size.y));
        surfaceTool.AddVertex(origin + BLOCK_SIZE * new Vector3(-0.5f, +0.5f, -0.5f));
    }

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
        material.AlbedoTexture = textureAtlas;
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

                    Vector3 blockPos = new Vector3(x, y, z);

                    Vector3 worldBlockPos = blockPos + new Vector3(chunkCoords.x * CHUNK_SIZE.x, 0, chunkCoords.y * CHUNK_SIZE.z);

                    int sx = (int) worldBlockPos.x;
                    int sy = (int) worldBlockPos.y;
                    int sz = (int) worldBlockPos.z;

                    Vector3 graphicalBlockPos = blockPos * BLOCK_SIZE;
                    
                    if(terrain.GetBlock(sx+1,sy,sz) == 0)
                        AddPosXFace(graphicalBlockPos, blockType);
                    if(terrain.GetBlock(sx-1,sy,sz) == 0)
                        AddNegXFace(graphicalBlockPos, blockType);
                    if(terrain.GetBlock(sx,sy+1,sz) == 0)
                        AddPosYFace(graphicalBlockPos, blockType);
                    if(terrain.GetBlock(sx,sy-1,sz) == 0 && y != 0)
                        AddNegYFace(graphicalBlockPos, blockType);
                    if(terrain.GetBlock(sx,sy,sz+1) == 0)
                        AddPosZFace(graphicalBlockPos, blockType);
                    if(terrain.GetBlock(sx,sy,sz-1) == 0)
                        AddNegZFace(graphicalBlockPos, blockType);
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
        blocks = worldGenerator.GetChunk(chunkCoords.x, chunkCoords.y, CHUNK_SIZE.x, CHUNK_SIZE.y, CHUNK_SIZE.z);
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

    public Chunk(Terrain terrain, WorldGenerator worldGenerator, Texture textureAtlas, Rect2[] textureUVs, IntVector2 coords)
    {
        this.terrain = terrain;
        this.Translate(new IntVector3((int) (coords.x * CHUNK_SIZE.x * BLOCK_SIZE), 0, (int) (coords.y * CHUNK_SIZE.z * BLOCK_SIZE)));
        this.worldGenerator = worldGenerator;
        this.textureAtlas = textureAtlas;
        this.textureUVs = textureUVs;

        this.chunkCoords = coords;
    }
    
    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here
        
    }

//    public override void _Process(float delta)
//    {
//        // Called every frame. Delta is time since last frame.
//        // Update game logic here.
//        
//    }
}
