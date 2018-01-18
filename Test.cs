using Godot;
using System;

public class Test : ImmediateGeometry
{
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here

        this.Begin(PrimitiveMesh.PrimitiveType.Triangles);
        this.AddVertex(new Vector3(0, 2, 0));
        this.AddVertex(new Vector3(2, 0, 0));
        this.AddVertex(new Vector3(-2, 0, 0));
        this.End();
        
    }

//    public override void _Process(float delta)
//    {
//        // Called every frame. Delta is time since last frame.
//        // Update game logic here.
//        
//    }
}
