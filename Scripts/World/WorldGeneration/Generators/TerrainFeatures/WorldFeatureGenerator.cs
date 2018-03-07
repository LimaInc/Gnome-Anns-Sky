using System;
using Godot;

public abstract class WorldFeatureGenerator
{
    public IntVector2 MaxSize { get; }
    public IntVector2 MinSize { get; }

    public WorldFeatureGenerator(IntVector2 minSize, IntVector2 maxSize)
    {
        MinSize = minSize;
        MaxSize = maxSize;
    }

    public abstract WorldFeature Generate(IntVector2 pos, IntVector2 boundingBoxSize, Random randGen);
}
