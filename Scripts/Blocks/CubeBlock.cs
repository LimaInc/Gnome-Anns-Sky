using Godot;

// TODO: is that performance critical code?
// if so, expaeriment with different setups for corners etc. 
// to see what works best
public abstract class CubeBlock : Block
{
    public static readonly Vector3[,,] corners = new Vector3[2,2,2];

    static CubeBlock()
    {
        for (int x = 0; x < 2; x ++)
        {
            for(int y = 0; y < 2; y ++)
            {
                for(int z = 0; z < 2; z ++)
                {
                    corners[x, y, z] = SIZE * (new Vector3(x, y, z) - 0.5f * new Vector3(1, 1, 1));
                }
            }
        }
    }

    private Vector3 this[int x, int y, int z] => corners[(x + 1) >> 1, (y + 1) >> 1, (z + 1) >> 1];

    // TODO: spend some time to actually properly refactor that (the first step is 25% done)
    public override void AddPosXFace(ref SurfaceTool surfaceTool, Vector3 origin, byte blockType)
    {
        Rect2 uvs = Game.GetBlockUV(blockType, BlockFace.Left);

        surfaceTool.AddUv(uvs.Get(1,1));
        surfaceTool.AddVertex(origin + this[+1, -1, -1]);
        surfaceTool.AddUv(uvs.Get(0, 0));
        surfaceTool.AddVertex(origin + this[+1, +1, +1]);
        surfaceTool.AddUv(uvs.Get(1, 0));
        surfaceTool.AddVertex(origin + this[+1, +1, -1]);

        surfaceTool.AddUv(uvs.Get(1, 1));
        surfaceTool.AddVertex(origin + this[+1, -1, -1]);
        surfaceTool.AddUv(uvs.Get(0, 1));
        surfaceTool.AddVertex(origin + this[+1, -1, +1]);
        surfaceTool.AddUv(uvs.Get(0, 0));
        surfaceTool.AddVertex(origin + this[+1, +1, +1]);
    }
    public override void AddNegXFace(ref SurfaceTool surfaceTool, Vector3 origin, byte blockType)
    {
        Rect2 uvs = Game.GetBlockUV(blockType, BlockFace.Right);

        surfaceTool.AddUv(uvs.Get(0, 1));
        surfaceTool.AddVertex(origin + this[-1, -1, -1]);
        surfaceTool.AddUv(uvs.Get(0, 0));
        surfaceTool.AddVertex(origin + this[-1, +1, -1]);
        surfaceTool.AddUv(uvs.Get(1, 0));
        surfaceTool.AddVertex(origin + this[-1, +1, +1]);

        surfaceTool.AddUv(uvs.Position + new Vector2(0, uvs.Size.y));
        surfaceTool.AddVertex(origin + SIZE * new Vector3(-0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(uvs.Size.x, 0));
        surfaceTool.AddVertex(origin + SIZE * new Vector3(-0.5f, +0.5f, +0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + SIZE * new Vector3(-0.5f, -0.5f, +0.5f));
    }
    public override void AddPosYFace(ref SurfaceTool surfaceTool, Vector3 origin, byte blockType)
    {
        Rect2 uvs = Game.GetBlockUV(blockType, BlockFace.Top);

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + SIZE * new Vector3(-0.5f, +0.5f, -0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(uvs.Size.x, 0));
        surfaceTool.AddVertex(origin + SIZE * new Vector3(+0.5f, +0.5f, -0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + SIZE * new Vector3(+0.5f, +0.5f, +0.5f));

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + SIZE * new Vector3(-0.5f, +0.5f, -0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + SIZE * new Vector3(+0.5f, +0.5f, +0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(0, uvs.Size.y));
        surfaceTool.AddVertex(origin + SIZE * new Vector3(-0.5f, +0.5f, +0.5f));
    }
    public override void AddNegYFace(ref SurfaceTool surfaceTool, Vector3 origin, byte blockType)
    {
        Rect2 uvs = Game.GetBlockUV(blockType, BlockFace.Bottom);

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + SIZE * new Vector3(-0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + SIZE * new Vector3(+0.5f, -0.5f, +0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(uvs.Size.x, 0));
        surfaceTool.AddVertex(origin + SIZE * new Vector3(+0.5f, -0.5f, -0.5f));

        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + SIZE * new Vector3(-0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(0, uvs.Size.y));
        surfaceTool.AddVertex(origin + SIZE * new Vector3(-0.5f, -0.5f, +0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + SIZE * new Vector3(+0.5f, -0.5f, +0.5f));
    }
    public override void AddPosZFace(ref SurfaceTool surfaceTool, Vector3 origin, byte blockType)
    {
        Rect2 uvs = Game.GetBlockUV(blockType, BlockFace.Front);

        surfaceTool.AddUv(uvs.Position + new Vector2(0, uvs.Size.y));
        surfaceTool.AddVertex(origin + SIZE * new Vector3(-0.5f, -0.5f, +0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(uvs.Size.x, 0));
        surfaceTool.AddVertex(origin + SIZE * new Vector3(+0.5f, +0.5f, +0.5f));
        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + SIZE * new Vector3(+0.5f, -0.5f, +0.5f));

        surfaceTool.AddUv(uvs.Position + new Vector2(0, uvs.Size.y));
        surfaceTool.AddVertex(origin + SIZE * new Vector3(-0.5f, -0.5f, +0.5f));
        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + SIZE * new Vector3(-0.5f, +0.5f, +0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(uvs.Size.x, 0));
        surfaceTool.AddVertex(origin + SIZE * new Vector3(+0.5f, +0.5f, +0.5f));
    }
    public override void AddNegZFace(ref SurfaceTool surfaceTool, Vector3 origin, byte blockType)
    {
        Rect2 uvs = Game.GetBlockUV(blockType, BlockFace.Back);

        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + SIZE * new Vector3(-0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(0, uvs.Size.y));
        surfaceTool.AddVertex(origin + SIZE * new Vector3(+0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + SIZE * new Vector3(+0.5f, +0.5f, -0.5f));

        surfaceTool.AddUv(uvs.End);
        surfaceTool.AddVertex(origin + SIZE * new Vector3(-0.5f, -0.5f, -0.5f));
        surfaceTool.AddUv(uvs.Position);
        surfaceTool.AddVertex(origin + SIZE * new Vector3(+0.5f, +0.5f, -0.5f));
        surfaceTool.AddUv(uvs.Position + new Vector2(uvs.Size.x, 0));
        surfaceTool.AddVertex(origin + SIZE * new Vector3(-0.5f, +0.5f, -0.5f));
    }
}