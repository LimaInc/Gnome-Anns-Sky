using Godot;
using System;
using System.Collections.Generic;

public class Interaction : Camera
{
    private PhysicsDirectSpaceState spaceState;

    private Terrain terrain;

    private Player player;

    public override void _Ready()
    {
        player = GetNode(Game.PLAYER_PATH) as Player;

        // Called every time the node is added to the scene.
        // Initialization here
        spaceState = GetWorld().DirectSpaceState;

        terrain = GetNode(Game.TERRAIN_PATH) as Terrain;

    }

    float rayLength = 5;

    public bool PlaceBlock(byte b)
    {
        Vector2 midScreenPoint = new Vector2(GetViewport().Size.x * 0.5f, GetViewport().Size.y * 0.5f);

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
                        return false;
                    }
                }
            }

            terrain.SetBlock(blockPos, b);
            return true;
        }
        return false;
    }

    public IntVector3? GetBlockPositionUnderCursor()
    {
        Vector2 midScreenPoint = new Vector2(GetViewport().Size.x * 0.5f, GetViewport().Size.y * 0.5f);

        Vector3 from = this.ProjectRayOrigin(midScreenPoint);
        Node[] exc = { this };
        Dictionary<object, object> hitInfo = spaceState.IntersectRay(from, from + this.ProjectRayNormal(midScreenPoint) * rayLength, exc);

        if(hitInfo.Count != 0) //Hit something
        {
            Vector3 pos = (Vector3)hitInfo["position"] - (Vector3)hitInfo["normal"] * 0.5f * Chunk.BLOCK_SIZE;
            IntVector3 blockPos = new IntVector3((int)Mathf.Round(pos.x / Chunk.BLOCK_SIZE), (int)Mathf.Round(pos.y / Chunk.BLOCK_SIZE), (int)Mathf.Round(pos.z / Chunk.BLOCK_SIZE));
            return blockPos;
        }
        return null;
    }

    public byte RemoveBlock()
    {
        if (!Game.isBreakable(GetBlock()))
            return 0;
            
        IntVector3? blockPossible = this.GetBlockPositionUnderCursor();
        
        if (blockPossible.HasValue)
        {
            IntVector3 blockPos = blockPossible.Value;
            byte ret = terrain.GetBlock(blockPos);
            terrain.SetBlock(blockPos, 0);
            return ret;
        }
        
        return 0;
    }

    public byte GetBlock()
    {
        IntVector3? blockPossible = this.GetBlockPositionUnderCursor();
        if (blockPossible.HasValue)
        {
            IntVector3 blockPos = blockPossible.Value;
            byte ret = terrain.GetBlock(blockPos);
            return ret;
        }

        return 0;
    }
}
