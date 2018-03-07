using Godot;

public class SmoothingTM : ITerrainModifier
{
    private readonly byte STONE_ID = Game.GetBlockId<Stone>();
    private readonly byte RED_ROCK_ID = Game.GetBlockId<RedRock>();
    private const byte AIR_ID = WorldGenerator.AIR_ID;

    private readonly Vector2 center;
    private readonly int centerHeight;

    private readonly float smoothingAtCenter;
    private readonly float halfSmoothingDistance;

    private readonly float sigmoidParamB;
    private readonly float sigmoidParamA;

    public SmoothingTM(Vector2 centerPos, int centerH, float smoothAtCenter, float halfSmoothingDist)
    {
        center = centerPos;
        centerHeight = centerH;

        smoothingAtCenter = smoothAtCenter;
        halfSmoothingDistance = halfSmoothingDist;

        sigmoidParamB = Mathf.Log(smoothingAtCenter / (1 - smoothingAtCenter));
        sigmoidParamA = -sigmoidParamB / halfSmoothingDistance;
    }

    public void UpdateHeight(Vector2 worldCoords, ref int height)
    {
        // probably overkill to do it for every single place in the world
        // TODO: think up a better solution
        // to do better we'd need to modify outside code
        // because maths here is almost as simple as possible
        float centerDist = (worldCoords - center).Length();
        float sigmoidSample = MathUtil.Sigmoid(centerDist, sigmoidParamA, sigmoidParamB);
        height = (int)Mathf.Round(sigmoidSample * centerHeight + (1 - sigmoidSample) * height);
    }
}
