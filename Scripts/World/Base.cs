using Godot;

public class Base : Node
{
    public const int DEFAULT_RADIUS = 8;
    public static readonly IntVector3 DEFAULT_POSITION = new IntVector3(-DEFAULT_RADIUS, 55, 0);

    private const float SMOOTHING = 0.99f;
    private const float halfSmoothingDistance = 2 * DEFAULT_RADIUS;

    public SmoothingGenerator SmoothingGenerator { get; private set; }
    public BaseGenerator Generator { get; private set; }

    public readonly float radius;
    public readonly IntVector3 position;
    
    public readonly int baseEntranceDepth;
    public readonly int baseFloorHeight;

    public Base(IntVector3? position = null, float radius = DEFAULT_RADIUS)
    {
        this.radius = radius;
        this.position = position ?? DEFAULT_POSITION;
        
        this.baseEntranceDepth = (int)this.radius / 4;
        this.baseFloorHeight = 2;

        SmoothingGenerator = 
            new SmoothingGenerator(new Vector2(this.position.x, this.position.z), this.position.y, SMOOTHING, halfSmoothingDistance);
        Generator = new BaseGenerator(this);
    }

    public bool IsGlobalPositionInside(Vector3 pos)
    {
        Vector3 localPos = pos - position;
        return pos.y >= baseFloorHeight &&
            localPos.Length() >= radius &&
            localPos.x < radius - baseEntranceDepth;
    }
}