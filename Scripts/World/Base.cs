using Godot;
using System.Diagnostics;

public class Base : Node
{
    public const int DEFAULT_RADIUS = 8;
    public static readonly IntVector3 DEFAULT_POSITION = new IntVector3(-DEFAULT_RADIUS, 55, 0);

    private const float SMOOTHING = 0.99f;
    private const float halfSmoothingDistance = 2 * DEFAULT_RADIUS;

    public SmoothingTM Smoothing { get; private set; }
    public BaseGenerator Generator { get; private set; }

    // hacky, should not be settable from the outside
    // might cause onflict with outside code accessing generators
    // TODO: find a better solution
    public IntVector3 position;
    public readonly float radius;

    public readonly int baseEntranceDepth;
    public readonly int domeOffset;

    // ugly, probably needed for Godot
    // TODO: fix
    public Base() : this(null) { }

    public Base(IntVector2 position, WorldGenerator wGen, float radius = DEFAULT_RADIUS) : this() { }

    public Base(IntVector3? position, float radius = DEFAULT_RADIUS)
    {
        this.radius = radius;
        this.position = position ?? DEFAULT_POSITION;

        this.domeOffset = 2;
        this.baseEntranceDepth = (int)this.radius / 4;
    }

    public void InitGeneration()
    {
        Smoothing =
            new SmoothingTM(new Vector2(this.position.x, this.position.z), this.position.y, SMOOTHING, halfSmoothingDistance);
        Generator = new BaseGenerator(this);
    }

    public bool IsGlobalPositionInside(Vector3 pos)
    {
        Vector3 localPos = pos / Block.SIZE - position;
        return localPos.y >= 0 &&
            (localPos + new Vector3(0, domeOffset, 0)).Length() <= radius &&
            localPos.x < radius - baseEntranceDepth;
    }
}