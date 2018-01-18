using Godot;
using System;

public class GenerateTerrain : MeshInstance
{
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";
    SurfaceTool surfaceTool = new SurfaceTool();

    private void AddPosXFace(Vector3 origin)
    {
        surfaceTool.AddUv(new Vector2(0, 0));
        surfaceTool.AddVertex(origin + new Vector3(0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(new Vector2(0.5f, 0.5f));
        surfaceTool.AddVertex(origin + new Vector3(0.5f, 0.5f, 0.5f));
        surfaceTool.AddUv(new Vector2(0.5f, 0));
        surfaceTool.AddVertex(origin + new Vector3(0.5f, 0.5f, -0.5f));

        surfaceTool.AddUv(new Vector2(0, 0));
        surfaceTool.AddVertex(origin + new Vector3(0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(new Vector2(0, 0.5f));
        surfaceTool.AddVertex(origin + new Vector3(0.5f, -0.5f, 0.5f));
        surfaceTool.AddUv(new Vector2(0.5f, 0.5f));
        surfaceTool.AddVertex(origin + new Vector3(0.5f, 0.5f, 0.5f));
    }
    private void AddNegXFace(Vector3 origin)
    {
        surfaceTool.AddUv(new Vector2(0, 0));
        surfaceTool.AddVertex(origin + new Vector3(-0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(new Vector2(0.5f, 0));
        surfaceTool.AddVertex(origin + new Vector3(-0.5f, 0.5f, -0.5f));
        surfaceTool.AddUv(new Vector2(0.5f, 0.5f));
        surfaceTool.AddVertex(origin + new Vector3(-0.5f, 0.5f, 0.5f));

        surfaceTool.AddUv(new Vector2(0, 0));
        surfaceTool.AddVertex(origin + new Vector3(-0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(new Vector2(0.5f, 0.5f));
        surfaceTool.AddVertex(origin + new Vector3(-0.5f, 0.5f, 0.5f));
        surfaceTool.AddUv(new Vector2(0, 0.5f));
        surfaceTool.AddVertex(origin + new Vector3(-0.5f, -0.5f, 0.5f));
    }
    private void AddPosYFace(Vector3 origin)
    {
        surfaceTool.AddUv(new Vector2(0.5f, 0.5f));
        surfaceTool.AddVertex(origin + new Vector3(-0.5f, 0.5f, -0.5f));
        surfaceTool.AddUv(new Vector2(1, 0.5f));
        surfaceTool.AddVertex(origin + new Vector3(0.5f, 0.5f, -0.5f));
        surfaceTool.AddUv(new Vector2(1, 1));
        surfaceTool.AddVertex(origin + new Vector3(0.5f, 0.5f, 0.5f));

        surfaceTool.AddUv(new Vector2(0.5f, 0.5f));
        surfaceTool.AddVertex(origin + new Vector3(-0.5f, 0.5f, -0.5f));
        surfaceTool.AddUv(new Vector2(1, 1));
        surfaceTool.AddVertex(origin + new Vector3(0.5f, 0.5f, 0.5f));
        surfaceTool.AddUv(new Vector2(0.5f, 1));
        surfaceTool.AddVertex(origin + new Vector3(-0.5f, 0.5f, 0.5f));
    }
    private void AddNegYFace(Vector3 origin)
    {
        surfaceTool.AddUv(new Vector2(0, 0));
        surfaceTool.AddVertex(origin + new Vector3(-0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(new Vector2(0, 0));
        surfaceTool.AddVertex(origin + new Vector3(0.5f, -0.5f, 0.5f));
        surfaceTool.AddUv(new Vector2(0, 0));
        surfaceTool.AddVertex(origin + new Vector3(0.5f, -0.5f, -0.5f));

        surfaceTool.AddUv(new Vector2(0, 0));
        surfaceTool.AddVertex(origin + new Vector3(-0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(new Vector2(0, 0));
        surfaceTool.AddVertex(origin + new Vector3(-0.5f, -0.5f, 0.5f));
        surfaceTool.AddUv(new Vector2(0, 0));
        surfaceTool.AddVertex(origin + new Vector3(0.5f, -0.5f, 0.5f));
    }
    private void AddPosZFace(Vector3 origin)
    {
        surfaceTool.AddUv(new Vector2(0, 0));
        surfaceTool.AddVertex(origin + new Vector3(-0.5f, -0.5f, 0.5f));
        surfaceTool.AddUv(new Vector2(0.5f, 0.5f));
        surfaceTool.AddVertex(origin + new Vector3(0.5f, 0.5f, 0.5f));
        surfaceTool.AddUv(new Vector2(0.5f, 0));
        surfaceTool.AddVertex(origin + new Vector3(0.5f, -0.5f, 0.5f));

        surfaceTool.AddUv(new Vector2(0, 0));
        surfaceTool.AddVertex(origin + new Vector3(-0.5f, -0.5f, 0.5f));
        surfaceTool.AddUv(new Vector2(0, 0.5f));
        surfaceTool.AddVertex(origin + new Vector3(-0.5f, 0.5f, 0.5f));
        surfaceTool.AddUv(new Vector2(0.5f, 0.5f));
        surfaceTool.AddVertex(origin + new Vector3(0.5f, 0.5f, 0.5f));
    }
    private void AddNegZFace(Vector3 origin)
    {
        surfaceTool.AddUv(new Vector2(0, 0));
        surfaceTool.AddVertex(origin + new Vector3(-0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(new Vector2(0.5f, 0));
        surfaceTool.AddVertex(origin + new Vector3(0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(new Vector2(0.5f, 0.5f));
        surfaceTool.AddVertex(origin + new Vector3(0.5f, 0.5f, -0.5f));

        surfaceTool.AddUv(new Vector2(0, 0));
        surfaceTool.AddVertex(origin + new Vector3(-0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(new Vector2(0.5f, 0.5f));
        surfaceTool.AddVertex(origin + new Vector3(0.5f, 0.5f, -0.5f));
        surfaceTool.AddUv(new Vector2(0, 0.5f));
        surfaceTool.AddVertex(origin + new Vector3(-0.5f, 0.5f, -0.5f));
    }

    byte[,,] blocks = new byte[32,32,32];

    private void BuildTerrain()
    {
        FastNoise noise = new FastNoise();
        for(int x = 0; x < blocks.GetLength(0); x++)
        {
            for(int z = 0; z < blocks.GetLength(2); z++)
            {
                int height = (int)(noise.GetSimplex(x * 4, z * 4) * 4) + 16;
                for(int y = 0; y < blocks.GetLength(1); y++)
                {
                    if(y < height)
                        blocks[x,y,z] = 1;
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
        
        ArrayMesh mesh = new ArrayMesh();
        SpatialMaterial material = new SpatialMaterial();
        Texture texture = ResourceLoader.Load("res://tilemap.png") as Texture;
        material.AlbedoTexture = texture;

        BuildTerrain();

        surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

        for(int x = 0; x < blocks.GetLength(0); x++)
        {
            for(int y = 0; y < blocks.GetLength(1); y++)
            {
                for(int z = 0; z < blocks.GetLength(2); z++)
                {
                    if(blocks[x,y,z] == 0)
                        continue;

                    Vector3 blockPos = new Vector3(x,y,z);

                    if(GetBlock(x+1,y,z) == 0)
                        AddPosXFace(blockPos);
                    if(GetBlock(x-1,y,z) == 0)
                        AddNegXFace(blockPos);
                    if(GetBlock(x,y+1,z) == 0)
                        AddPosYFace(blockPos);
                    if(GetBlock(x,y-1,z) == 0)
                        AddNegYFace(blockPos);
                    if(GetBlock(x,y,z+1) == 0)
                        AddPosZFace(blockPos);
                    if(GetBlock(x,y,z-1) == 0)
                        AddNegZFace(blockPos);
                }
            }
        }

        surfaceTool.GenerateNormals();

        surfaceTool.SetMaterial(material);

        mesh = surfaceTool.Commit();
        this.SetMesh(mesh);
    }

//    public override void _Process(float delta)
//    {
//        // Called every frame. Delta is time since last frame.
//        // Update game logic here.
//        
//    }
}
