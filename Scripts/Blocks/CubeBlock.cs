using Godot;

public abstract class CubeBlock : Block
{
    public override void AddPosXFace(ref SurfaceTool surfaceTool, Vector3 origin, byte blockType)
    {
        Rect2 uvs = Game.GetBlockUV(blockType, BlockFace.Left);

        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(+0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(+0.5f, +0.5f, +0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(uvs.Size.x, 0));
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(+0.5f, +0.5f, -0.5f));

        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(+0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(0, uvs.Size.y));
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(+0.5f, -0.5f, +0.5f));
        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(+0.5f, +0.5f, +0.5f));
    }
    public override void AddNegXFace(ref SurfaceTool surfaceTool, Vector3 origin, byte blockType)
    {
        Rect2 uvs = Game.GetBlockUV(blockType, BlockFace.Right);

        surfaceTool.AddUv(uvs.Position + new Vector2(0, uvs.Size.y));
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(-0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(-0.5f, +0.5f, -0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(uvs.Size.x, 0));
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(-0.5f, +0.5f, +0.5f));

        surfaceTool.AddUv(uvs.Position + new Vector2(0, uvs.Size.y));
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(-0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(uvs.Size.x, 0));
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(-0.5f, +0.5f, +0.5f));
        surfaceTool.AddUv(uvs.End);
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

        surfaceTool.AddUv(uvs.Position + new Vector2(0, uvs.Size.y));
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(-0.5f, -0.5f, +0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(uvs.Size.x, 0));
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(+0.5f, +0.5f, +0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(+0.5f, -0.5f, +0.5f));

        surfaceTool.AddUv(uvs.Position + new Vector2(0, uvs.Size.y));
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(-0.5f, -0.5f, +0.5f));
        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(-0.5f, +0.5f, +0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(uvs.Size.x, 0));
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(+0.5f, +0.5f, +0.5f));
    }
    public override void AddNegZFace(ref SurfaceTool surfaceTool, Vector3 origin, byte blockType)
    {
        Rect2 uvs = Game.GetBlockUV(blockType, BlockFace.Back);

        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(-0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(0, uvs.Size.y));
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(+0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(+0.5f, +0.5f, -0.5f));

        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(-0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(+0.5f, +0.5f, -0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(uvs.Size.x, 0));
        surfaceTool.AddVertex(origin + Chunk.BLOCK_SIZE * new Vector3(-0.5f, +0.5f, -0.5f));
    }
}