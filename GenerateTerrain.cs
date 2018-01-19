using Godot;
using System;

public class GenerateTerrain : Spatial
{
    SurfaceTool surfaceTool = new SurfaceTool();

    //Rect at index i tells you the UVs for block i+1 (because air=0 and has no texture)
    //Hardcoded for now, but we might want a better solution. Perhaps automatic texture packer?
    //Probably want to support multiple textures for a single block at different rotations, e.g. grass
    Rect2[] blockUVs = {
        new Rect2(0.0f, 0.0f, 0.5f, 1.0f),
        new Rect2(0.5f, 0.0f, 0.5f, 1.0f)
    };

    private void AddPosXFace(Vector3 origin, byte blockType)
    {
        Rect2 uvs = blockUVs[blockType-1];

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
        Rect2 uvs = blockUVs[blockType-1];

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
        Rect2 uvs = blockUVs[blockType-1];

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
        Rect2 uvs = blockUVs[blockType-1];

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
        Rect2 uvs = blockUVs[blockType-1];

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
        Rect2 uvs = blockUVs[blockType-1];

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

    byte[,,] blocks = new byte[32,32,32];

    private void BuildTerrain()
    {
        FastNoise noise = new FastNoise();
        Random r = new Random();
        for(int x = 0; x < blocks.GetLength(0); x++)
        {
            for(int z = 0; z < blocks.GetLength(2); z++)
            {
                int height = (int)(noise.GetSimplex(x * 4, z * 4) * 4) + 16;
                for(int y = 0; y < blocks.GetLength(1); y++)
                {
                    if(y < height)
                        blocks[x,y,z] = (byte)r.Next(1, 3);
                    else
                        blocks[x,y,z] = 0;
                }
            }
        }
    }

    byte GetBlock(int x, int y, int z)
    {
        if(x < 0 || x >= blocks.GetLength(0) || y < 0 || y >= blocks.GetLength(1) || z < 0 || z >= blocks.GetLength(2))
            return 0;
        else
            return blocks[x,y,z];
    }

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here

        MeshInstance meshInstance= new MeshInstance();
        this.AddChild(meshInstance);

        StaticBody body = new StaticBody();
        this.AddChild(body);
        CollisionShape collider = new CollisionShape();
        body.AddChild(collider);

        ConcavePolygonShape shape = new ConcavePolygonShape();
        collider.Shape = shape;
        
        ArrayMesh mesh = new ArrayMesh();
        SpatialMaterial material = new SpatialMaterial();
        Texture atlas = ResourceLoader.Load("res://tilemap.png") as Texture;
        material.AlbedoTexture = atlas;

        BuildTerrain();

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

                    if(GetBlock(x+1,y,z) == 0)
                        AddPosXFace(blockPos, blockType);
                    if(GetBlock(x-1,y,z) == 0)
                        AddNegXFace(blockPos, blockType);
                    if(GetBlock(x,y+1,z) == 0)
                        AddPosYFace(blockPos, blockType);
                    if(GetBlock(x,y-1,z) == 0)
                        AddNegYFace(blockPos, blockType);
                    if(GetBlock(x,y,z+1) == 0)
                        AddPosZFace(blockPos, blockType);
                    if(GetBlock(x,y,z-1) == 0)
                        AddNegZFace(blockPos, blockType);
                }
            }
        }

        surfaceTool.GenerateNormals();

        surfaceTool.SetMaterial(material);

        mesh = surfaceTool.Commit();

        shape.Data = mesh.GetFaces();

        meshInstance.SetMesh(mesh);
    }

//    public override void _Process(float delta)
//    {
//        // Called every frame. Delta is time since last frame.
//        // Update game logic here.
//        
//    }
}
