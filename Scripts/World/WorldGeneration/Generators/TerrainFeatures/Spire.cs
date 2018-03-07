using Godot;

public class Spire : WorldFeature
{
    private float PosY { get; }
    // horizontal radius first, i.e. .x
    private Vector2 BaseRadii { get; }
    private float ConeHeight { get; }
    private float Inclination { get; }
    private byte Material { get; }

    public Spire(Vector3 pos, Vector2 boundingRect, float rotAngle, Vector2 baseRadii, float coneHeight, float inclination, byte material) : 
        base(new Vector2(pos.x, pos.z), boundingRect, rotAngle)
    {
        PosY = pos.y;
        BaseRadii = baseRadii;
        ConeHeight = coneHeight;
        Inclination = inclination;
        Material = material;
    }

    public override byte? BlockAt(int terrainHeight, IntVector3 globalPos)
    {
        Vector3 featureLocalPos = ToLocal(globalPos);
        Vector3 coneLocalPos = featureLocalPos.Rotated(new Vector3(0, 0, 1), Inclination) - new Vector3(0, BaseRadii.y, BaseRadii.x) / 2;
        if (IsInside(coneLocalPos))
        {
            return Material;
        }
        else
        {
            return null;
        }
    }

    public bool IsInside(Vector3 localPos)
    {
        if (localPos.x >= 0 && localPos.x <= ConeHeight)
        {
            return (new Vector2(localPos.z, localPos.y) / BaseRadii).LengthSquared() <= Mathf.Pow(1 - localPos.x / ConeHeight, 2);
        }
        else
        {
            return false;
        }
    }
}
