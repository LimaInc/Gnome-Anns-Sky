using System;
using System.Collections.Generic;

public abstract class PlantManager
{
    protected Atmosphere atmosphere;
    protected Terrain terrain;
    protected HashSet<IntVector3> plantBlocks;
    protected HashSet<IntVector3> plantActiveBlocks;

    protected const float LIFECYCLE_TICK_TIME = 1f;
    protected readonly double spreadChance;
    protected readonly IDictionary<Gas, float> gasDeltas;

    static protected Random randGen = new Random();

    public PlantManager(Plants plants, double spreadChance_, IDictionary<Gas,float> gasDeltas_)
    {
        atmosphere = plants.atmosphere;
        terrain = plants.terrain;
        plantBlocks = new HashSet<IntVector3>();
        plantActiveBlocks = new HashSet<IntVector3>();
        spreadChance = spreadChance_;
        gasDeltas = gasDeltas_;
    }

    abstract protected bool CanSpreadTo(IntVector3 blockPos);
    abstract public bool PlantOn(IntVector3 blockPos);

    abstract public void LifeCycle(float delta);
    abstract protected void Spread();

    public void AdjustGases(float delta)
    {
        foreach (KeyValuePair<Gas, float> entry in gasDeltas)
            atmosphere.ChangeGasAmt(entry.Key, entry.Value * plantBlocks.Count);
    }
}
