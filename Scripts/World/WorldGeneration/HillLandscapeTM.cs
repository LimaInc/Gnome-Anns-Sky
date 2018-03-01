using Godot;

public class HillLandscapeTM : ITerrainModifier
{
    OctaveNoise noise = new OctaveNoise(16);

    public readonly int averageHeight;
    public readonly int heightSpread;

    public HillLandscapeTM(int averageHeight, int heightSpread)
    {
        this.averageHeight = averageHeight;
        this.heightSpread = heightSpread;
    }

    public void UpdateHeight(Vector2 worldCoords, ref int height)
    {
        Vector2 fractionalCoords = worldCoords / 256;
        height = (int) (noise.Sample(fractionalCoords.x, fractionalCoords.y) * heightSpread) + averageHeight;
    }
}
