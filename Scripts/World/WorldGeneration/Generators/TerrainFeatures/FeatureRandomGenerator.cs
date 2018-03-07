using System;
using Godot;

public class FeatureRandomGenerator
{
    // possible optimisation: dynamic programming

    private IntVector2 MajorAreaSize { get; }
    private IntVector2 MaxMinorAreas { get; }

    private WorldFeatureGenerator FeatureGenerator { get; set; }

    private int Seed { get; }

    public FeatureRandomGenerator(WorldFeatureGenerator fGen)
    {
        FeatureGenerator = fGen;
        int maxFeatureRadius = (int)fGen.MaxSize.Length();
        // implementation choice: all major areas are square
        // and max numbers of minor areas are the same for both dimensions
        MajorAreaSize = new IntVector2(maxFeatureRadius, maxFeatureRadius);
        MaxMinorAreas = MajorAreaSize / (int)fGen.MinSize.Length();

        Seed = MathUtil.GlobalRandom.Next();
    }

    private IntVector2 GetMajorAreaIndex(IntVector2 pos)
    {
        return pos / MajorAreaSize;
    }

    private IntVector2 GetSplitLevelFor(IntVector2 majorAreaIndex)
    {
        Random r = new Random(Tuple.Create(Seed, majorAreaIndex).GetHashCode());
        return new IntVector2(1 + r.Next() % MaxMinorAreas.x, 1 + r.Next() % MaxMinorAreas.y);
    }

    private Tuple<IntVector2,IntVector2> GetMinorAreaDimIndex(IntVector2 pos)
    {
        IntVector2 majorAreaIndex = GetMajorAreaIndex(pos);
        IntVector2 splitLevel = GetSplitLevelFor(majorAreaIndex);
        IntVector2 localPos = pos - majorAreaIndex * MajorAreaSize;
        IntVector2 minorAreaSize = MajorAreaSize / splitLevel;
        return Tuple.Create(minorAreaSize, localPos / minorAreaSize);
    }

    private WorldFeature GetWorldFeatureForPosition(IntVector2 pos, int terrainHeight)
    {
        IntVector2 majorAreaIndex = GetMajorAreaIndex(pos);
        Tuple<IntVector2, IntVector2> minorAreaData = GetMinorAreaDimIndex(pos);
        IntVector2 minorAreaSize = minorAreaData.Item1;
        IntVector2 minorAreaIndex = minorAreaData.Item2;
        Random r = new Random(Tuple.Create(Seed, minorAreaIndex, majorAreaIndex).GetHashCode());
        // TODO: fix 
        return FeatureGenerator.Generate(majorAreaIndex * MajorAreaSize + minorAreaIndex * minorAreaSize, terrainHeight, minorAreaSize, r);
    }
}
