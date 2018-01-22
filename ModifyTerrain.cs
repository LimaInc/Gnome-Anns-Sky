using Godot;
using System;
using System.Collections.Generic;

public class ModifyTerrain : Camera
{
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";

    PhysicsDirectSpaceState spaceState;

    Terrain terrain;

    Vector2 midScreenPoint;

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here
        spaceState = GetWorld().DirectSpaceState;

        terrain = GetNode("/root/Node/Terrain") as Terrain;

        midScreenPoint = new Vector2(GetViewport().Size.x * 0.5f, GetViewport().Size.y * 0.5f);
    }

    bool removeBlock = false;
    public override void _Input(InputEvent e)
    {
        if(e is InputEventMouseButton)
        {
            InputEventMouseButton inputEvent = (InputEventMouseButton)e;

            if(inputEvent.ButtonIndex == 1 && inputEvent.Pressed) //Only triggers on button down
                removeBlock = true;
        }
    }

    float rayLength = 5;
    public override void _PhysicsProcess(float delta)
    {
        if(removeBlock)
        {
            removeBlock = false;

            Vector3 from = this.ProjectRayOrigin(midScreenPoint);
            Dictionary<object, object> hitInfo = spaceState.IntersectRay(from, from + this.ProjectRayNormal(midScreenPoint) * rayLength);

            if(hitInfo.Count != 0) //Hit something
            {
                Vector3 pos = (Vector3)hitInfo["position"] - (Vector3)hitInfo["normal"] * 0.5f;
                IntVector3 blockPos = new IntVector3((int)Mathf.Round(pos.x), (int)Mathf.Round(pos.y), (int)Mathf.Round(pos.z));

                terrain.SetBlock(blockPos, 0);
            }
        }
    }

//    public override void _Process(float delta)
//    {
//        // Called every frame. Delta is time since last frame.
//        // Update game logic here.
//        
//    }
}
