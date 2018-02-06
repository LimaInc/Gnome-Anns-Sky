using Godot;
using System;
using System.Collections.Generic;

public class Interaction : Camera
{
    private PhysicsDirectSpaceState spaceState;

    private Terrain terrain;

    private Player player;

    public static Vector2 midScreenPoint;

    public override void _Ready()
    {
        player = GetNode("/root/Node/Player") as Player;

        // Called every time the node is added to the scene.
        // Initialization here
        spaceState = GetWorld().DirectSpaceState;

        terrain = GetNode("/root/Node/Terrain") as Terrain;

    }

    bool removeBlock = false;
    bool placeBlock = false;
    public override void _Input(InputEvent e)
    {
        if(e is InputEventMouseButton && !player.isInventoryOpen())
        {
            InputEventMouseButton inputEvent = (InputEventMouseButton)e;

            if (inputEvent.ButtonIndex == 1 && inputEvent.Pressed) //Only triggers on button down
                removeBlock = true;

            if (inputEvent.ButtonIndex == 2 && inputEvent.Pressed)
                placeBlock = true;
        }
    }

    float rayLength = 5;
    public override void _PhysicsProcess(float delta)
    {
        midScreenPoint = new Vector2(GetViewport().Size.x * 0.5f, GetViewport().Size.y * 0.5f);
        if(removeBlock)
        {
            removeBlock = false;

            Vector3 from = this.ProjectRayOrigin(midScreenPoint);
            Node[] exc = { this };
            Dictionary<object, object> hitInfo = spaceState.IntersectRay(from, from + this.ProjectRayNormal(midScreenPoint) * rayLength, exc);

            if(hitInfo.Count != 0) //Hit something
            {
                Vector3 pos = (Vector3)hitInfo["position"] - (Vector3)hitInfo["normal"] * 0.5f * Chunk.BLOCK_SIZE;
                IntVector3 blockPos = new IntVector3((int)Mathf.Round(pos.x / Chunk.BLOCK_SIZE), (int)Mathf.Round(pos.y / Chunk.BLOCK_SIZE), (int)Mathf.Round(pos.z / Chunk.BLOCK_SIZE));

                terrain.SetBlock(blockPos, 0);
            }
        }
        if (placeBlock)
        {
            placeBlock = false;

            Vector3 from = this.ProjectRayOrigin(midScreenPoint);
            Node[] exc = { this };
            Dictionary<object, object> hitInfo = spaceState.IntersectRay(from, from + this.ProjectRayNormal(midScreenPoint) * rayLength, exc);

            foreach(KeyValuePair<object, object> entry in hitInfo)
            {
                // do something with entry.Value or entry.Key
            }

            if(hitInfo.Count != 0) //Hit something
            {
                Vector3 pos = (Vector3)hitInfo["position"] + (Vector3)hitInfo["normal"] * 0.5f * Chunk.BLOCK_SIZE;
                IntVector3 blockPos = new IntVector3((int)Mathf.Round(pos.x / Chunk.BLOCK_SIZE), (int)Mathf.Round(pos.y / Chunk.BLOCK_SIZE), (int)Mathf.Round(pos.z / Chunk.BLOCK_SIZE));

                Vector3 blockCollisionPos = new Vector3(blockPos.x, blockPos.y, blockPos.z) * Chunk.BLOCK_SIZE;

                BoxShape bs = new BoxShape();
                bs.SetExtents(new Vector3(Chunk.BLOCK_SIZE / 2.0f, Chunk.BLOCK_SIZE / 2.0f, Chunk.BLOCK_SIZE / 2.0f));

                PhysicsShapeQueryParameters psqp = new PhysicsShapeQueryParameters();
                psqp.SetShape(bs);
                Transform t = new Transform(new Vector3(1.0f, 0.0f, 0.0f),new Vector3(0.0f, 1.0f, 0.0f),new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f ,0.0f)).Translated(blockCollisionPos);
                psqp.SetTransform(t);

                object[] res = spaceState.IntersectShape(psqp);

                if (res.Length > 0)
                {
                    for (int i = 0; i < res.Length; i++)
                    {
                        Dictionary<object,object> info = (Dictionary<object,object>) spaceState.IntersectShape(psqp)[i];

                        if (info["collider"] is KinematicBody)
                        {
                            //A moving body (player, animal etc.) is in the way
                            return;
                        }
                    }
                }

                terrain.SetBlock(blockPos, 1);
            }
        }
    }
}