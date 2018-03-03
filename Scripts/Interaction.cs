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
        var hitInfo = GetHitInfo();

        if (hitInfo != null)
        {
            Vector3 pos = (Vector3)hitInfo["position"] + (Vector3)hitInfo["normal"] * 0.5f * Block.SIZE;
            IntVector3 blockPos = new IntVector3((int)Mathf.Round(pos.x / Block.SIZE), (int)Mathf.Round(pos.y / Block.SIZE), (int)Mathf.Round(pos.z / Block.SIZE));

            Vector3 blockCollisionPos = new Vector3(blockPos.x, blockPos.y, blockPos.z) * Block.SIZE;

            BoxShape bs = new BoxShape();
            bs.SetExtents(new Vector3(Block.SIZE, Block.SIZE, Block.SIZE) / 2);

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
                        // A moving body (player, animal etc.) is in the way
                        return false;
                    }
                }
            }

            terrain.SetBlock(blockPos, b);
            return true;
        }
        return false;
    }

    public Dictionary<object, object> GetHitInfo()
    {
        Vector2 midScreenPoint = new Vector2(GetViewport().Size.x * 0.5f, GetViewport().Size.y * 0.5f);

        Vector3 from = ProjectRayOrigin(midScreenPoint);
        Node[] exc = { this };
        Dictionary<object, object> hitInfo = spaceState.IntersectRay(from, from + ProjectRayNormal(midScreenPoint) * rayLength, exc);

        return hitInfo.Count > 0 ? hitInfo : null;
    }

    public IntVector3? GetBlockPositionUnderCursor()
    {
        var hitInfo = GetHitInfo();

        if(hitInfo != null) //Hit something
        {
            Vector3 pos = (Vector3)hitInfo["position"] - (Vector3)hitInfo["normal"] * 0.5f * Block.SIZE;
            IntVector3 blockPos = new IntVector3((int)Mathf.Round(pos.x / Block.SIZE), (int)Mathf.Round(pos.y / Block.SIZE), (int)Mathf.Round(pos.z / Block.SIZE));
            return blockPos;
        }
        return null;
    }

    public byte RemoveBlock()
    {
        IntVector3? blockPossible = GetBlockPositionUnderCursor();

        if (blockPossible.HasValue)
        {
            IntVector3 blockPos = blockPossible.Value;
            
            byte ret = terrain.GetBlock(blockPos);

            if (!Game.GetBlock(ret).Breakable)
                return 0;
            
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
