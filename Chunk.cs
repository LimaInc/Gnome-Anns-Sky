using Godot;
using System;

public class Chunk : Spatial
{
    private byte[,,] blocks;
    private SurfaceTool surfaceTool = new SurfaceTool();
    private WorldGenerator worldGenerator;

    private MeshInstance meshInstance;
    private StaticBody body;
    private CollisionShape collider;

    private void AddPosXFace(Vector3 origin, byte blockType)
    {
        Rect2 uvs = worldGenerator.blockUVs[blockType-1];

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + new Vector3(0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + new Vector3(0.5f, 0.5f, 0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(uvs.Size.x, 0));
        surfaceTool.AddVertex(origin + new Vector3(0.5f, 0.5f, -0.5f));

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + new Vector3(0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(0, uvs.Size.y));
        surfaceTool.AddVertex(origin + new Vector3(0.5f, -0.5f, 0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + new Vector3(0.5f, 0.5f, 0.5f));
    }
    private void AddNegXFace(Vector3 origin, byte blockType)
    {
        Rect2 uvs = worldGenerator.blockUVs[blockType-1];

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + new Vector3(-0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(uvs.Size.x, 0));
        surfaceTool.AddVertex(origin + new Vector3(-0.5f, 0.5f, -0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + new Vector3(-0.5f, 0.5f, 0.5f));

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + new Vector3(-0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + new Vector3(-0.5f, 0.5f, 0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(0, uvs.Size.y));
        surfaceTool.AddVertex(origin + new Vector3(-0.5f, -0.5f, 0.5f));
    }
    private void AddPosYFace(Vector3 origin, byte blockType)
    {
        Rect2 uvs = worldGenerator.blockUVs[blockType-1];

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + new Vector3(-0.5f, 0.5f, -0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(uvs.Size.x, 0));
        surfaceTool.AddVertex(origin + new Vector3(0.5f, 0.5f, -0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + new Vector3(0.5f, 0.5f, 0.5f));

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + new Vector3(-0.5f, 0.5f, -0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + new Vector3(0.5f, 0.5f, 0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(0, uvs.Size.y));
        surfaceTool.AddVertex(origin + new Vector3(-0.5f, 0.5f, 0.5f));
    }
    private void AddNegYFace(Vector3 origin, byte blockType)
    {
        Rect2 uvs = worldGenerator.blockUVs[blockType-1];

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + new Vector3(-0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(uvs.Size.x, 0));
        surfaceTool.AddVertex(origin + new Vector3(0.5f, -0.5f, 0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + new Vector3(0.5f, -0.5f, -0.5f));

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + new Vector3(-0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + new Vector3(-0.5f, -0.5f, 0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(0, uvs.Size.y));
        surfaceTool.AddVertex(origin + new Vector3(0.5f, -0.5f, 0.5f));
    }
    private void AddPosZFace(Vector3 origin, byte blockType)
    {
        Rect2 uvs = worldGenerator.blockUVs[blockType-1];

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + new Vector3(-0.5f, -0.5f, 0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(uvs.Size.x, 0));
        surfaceTool.AddVertex(origin + new Vector3(0.5f, 0.5f, 0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + new Vector3(0.5f, -0.5f, 0.5f));

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + new Vector3(-0.5f, -0.5f, 0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + new Vector3(-0.5f, 0.5f, 0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(0, uvs.Size.y));
        surfaceTool.AddVertex(origin + new Vector3(0.5f, 0.5f, 0.5f));
    }
    private void AddNegZFace(Vector3 origin, byte blockType)
    {
        Rect2 uvs = worldGenerator.blockUVs[blockType-1];

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + new Vector3(-0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(uvs.Size.x, 0));
        surfaceTool.AddVertex(origin + new Vector3(0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + new Vector3(0.5f, 0.5f, -0.5f));

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + new Vector3(-0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + new Vector3(0.5f, 0.5f, -0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(0, uvs.Size.y));
        surfaceTool.AddVertex(origin + new Vector3(-0.5f, 0.5f, -0.5f));
    }

    public byte GetBlockInChunk(IntVector3 position)
    {
        return GetBlockInChunk(position.x, position.y, position.z);
    }

    public byte GetBlockInChunk(int x, int y, int z)
    {
        if(x < 0 || x >= blocks.GetLength(0) || y < 0 || y >= blocks.GetLength(1) || z < 0 || z >= blocks.GetLength(2))
            return 0; //Maybe should throw exception/return null here ??
        else
            return blocks[x,y,z];
    }

    public void SetBlockInChunk(int x, int y, int z, byte block)
    {
        if(x < 0 || x >= blocks.GetLength(0) || y < 0 || y >= blocks.GetLength(1) || z < 0 || z >= blocks.GetLength(2))
            return; //Maybe should return false here?
        else
            blocks[x,y,z] = block;
    }

    public void UpdateMesh()
    {
        ArrayMesh mesh = new ArrayMesh();
        SpatialMaterial material = new SpatialMaterial();
        Texture atlas = ResourceLoader.Load("res://tilemap.png") as Texture;
        material.AlbedoTexture = atlas;

        surfaceTool.Begin(Mesh.PrimitiveType.Triangles);        

        for(int x = 0; x < blocks.GetLength(0); x++)
        {
            for(int y = 0; y < blocks.GetLength(1); y++)
            {
                for(int z = 0; z < blocks.GetLength(2); z++)
                {
                    byte blockType = blocks[x,y,z];
                    if(blockType == 0)
                        continue;

                    Vector3 blockPos = new Vector3(x,y,z);

                    if(GetBlockInChunk(x+1,y,z) == 0)
                        AddPosXFace(blockPos, blockType);
                    if(GetBlockInChunk(x-1,y,z) == 0)
                        AddNegXFace(blockPos, blockType);
                    if(GetBlockInChunk(x,y+1,z) == 0)
                        AddPosYFace(blockPos, blockType);
                    if(GetBlockInChunk(x,y-1,z) == 0)
                        AddNegYFace(blockPos, blockType);
                    if(GetBlockInChunk(x,y,z+1) == 0)
                        AddPosZFace(blockPos, blockType);
                    if(GetBlockInChunk(x,y,z-1) == 0)
                        AddNegZFace(blockPos, blockType);
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

    public Chunk(WorldGenerator worldGenerator, IntVector3 position, IntVector3 size)
    {
        this.Translate(position);
        this.worldGenerator = worldGenerator;
        blocks = worldGenerator.GetChunk(position.x, position.y, position.z, size.x, size.y, size.z);

        meshInstance = new MeshInstance();
        this.AddChild(meshInstance);

        body = new StaticBody();
        this.AddChild(body);
        collider = new CollisionShape();
        body.AddChild(collider);
        
        UpdateMesh();
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
