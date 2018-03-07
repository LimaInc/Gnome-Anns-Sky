using Godot;

public abstract class WorldFeature
{
    public Vector2 Position { get; }
    public Vector2 Size { get; }
    public float Angle { get; }

    public WorldFeature(Vector2 pos, Vector2 size, float angle)
    {
        Position = pos;
        Size = size;
        Angle = angle;
    }

    public abstract byte? BlockAt(int terrainHeight, IntVector3 globalPos);

    protected Vector3 ToLocal(IntVector3 globalPos)
    {
        return new Vector3(globalPos.x - Position.x, globalPos.y, globalPos.z - Position.y).Rotated(new Vector3(0,1,0), -Angle);
    }
}
