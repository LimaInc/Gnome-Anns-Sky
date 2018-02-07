using System;
using System.Collections.Generic;
using Godot;

public enum BlockFace
{
    Top,
    Bottom,
    Left,
    Right,
    Front,
    Back
}
public abstract class Block
{
    abstract public string[] TexturePaths { get; }
    abstract public int GetTextureIndex(BlockFace face);

    abstract public void AddPosXFace(ref SurfaceTool surfaceTool, Vector3 origin, byte blockType);
    abstract public void AddNegXFace(ref SurfaceTool surfaceTool, Vector3 origin, byte blockType);
    abstract public void AddPosYFace(ref SurfaceTool surfaceTool, Vector3 origin, byte blockType);
    abstract public void AddNegYFace(ref SurfaceTool surfaceTool, Vector3 origin, byte blockType);
    abstract public void AddPosZFace(ref SurfaceTool surfaceTool, Vector3 origin, byte blockType);
    abstract public void AddNegZFace(ref SurfaceTool surfaceTool, Vector3 origin, byte blockType);

}

public abstract class CubeBlock : Block
{
    public override void AddPosXFace(ref SurfaceTool surfaceTool, Vector3 origin, byte blockType)
    {
        Rect2 uvs = Game.GetBlockUV(blockType, BlockFace.Left);

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(+0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(+0.5f, +0.5f, +0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(uvs.Size.x, 0));
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(+0.5f, +0.5f, -0.5f));

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(+0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(0, uvs.Size.y));
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(+0.5f, -0.5f, +0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(+0.5f, +0.5f, +0.5f));
    }
    public override void AddNegXFace(ref SurfaceTool surfaceTool, Vector3 origin, byte blockType)
    {
        Rect2 uvs = Game.GetBlockUV(blockType, BlockFace.Right);

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(-0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(uvs.Size.x, 0));
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(-0.5f, +0.5f, -0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(-0.5f, +0.5f, +0.5f));

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(-0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(-0.5f, +0.5f, +0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(0, uvs.Size.y));
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(-0.5f, -0.5f, +0.5f));
    }
    public override void AddPosYFace(ref SurfaceTool surfaceTool, Vector3 origin, byte blockType)
    {
        Rect2 uvs = Game.GetBlockUV(blockType, BlockFace.Top);

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(-0.5f, +0.5f, -0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(uvs.Size.x, 0));
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(+0.5f, +0.5f, -0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(+0.5f, +0.5f, +0.5f));

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(-0.5f, +0.5f, -0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(+0.5f, +0.5f, +0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(0, uvs.Size.y));
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(-0.5f, +0.5f, +0.5f));
    }
    public override void AddNegYFace(ref SurfaceTool surfaceTool, Vector3 origin, byte blockType)
    {
        Rect2 uvs = Game.GetBlockUV(blockType, BlockFace.Bottom);

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(-0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(+0.5f, -0.5f, +0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(uvs.Size.x, 0));
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(+0.5f, -0.5f, -0.5f));

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(-0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(0, uvs.Size.y));
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(-0.5f, -0.5f, +0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(+0.5f, -0.5f, +0.5f));
    }
    public override void AddPosZFace(ref SurfaceTool surfaceTool, Vector3 origin, byte blockType)
    {
        Rect2 uvs = Game.GetBlockUV(blockType, BlockFace.Front);

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(-0.5f, -0.5f, +0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(+0.5f, +0.5f, +0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(uvs.Size.x, 0));
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(+0.5f, -0.5f, +0.5f));

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(-0.5f, -0.5f, +0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(0, uvs.Size.y));
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(-0.5f, +0.5f, +0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(+0.5f, +0.5f, +0.5f));
    }
    public override void AddNegZFace(ref SurfaceTool surfaceTool, Vector3 origin, byte blockType)
    {
        Rect2 uvs = Game.GetBlockUV(blockType, BlockFace.Back);

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(-0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(uvs.Size.x, 0));
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(+0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(+0.5f, +0.5f, -0.5f));

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(-0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(+0.5f, +0.5f, -0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(0, uvs.Size.y));
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(-0.5f, +0.5f, -0.5f));
    }
}
public class Stone : CubeBlock
{
    public override string[] TexturePaths { get { return new[] { "res://Images/stone.png" }; } }

    public override int GetTextureIndex(BlockFace face)
    {
        return 0;
    }
}
public class Grass : CubeBlock
{
    public override string[] TexturePaths { get { return new[] { "res://Images/brown_rock_top.png", "res://Images/brown_rock_side.png" }; } }

    public override int GetTextureIndex(BlockFace face)
    {
        switch(face)
        {
            case BlockFace.Top:
                return 0;
            default:
                return 1;
        }
    }
}