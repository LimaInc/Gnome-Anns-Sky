using System;

public class SpireGenerator : WorldFeatureGenerator
{
    public SpireGenerator(IntVector2 minSize, IntVector2 maxSize) : base(minSize, maxSize)
    {
    }

    public override WorldFeature Generate(IntVector2 pos, IntVector2 boundingBoxSize, Random randGen)
    {
        return new Spire()
    }
}
