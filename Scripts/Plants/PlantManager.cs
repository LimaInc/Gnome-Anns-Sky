using System;
using System.Collections.Generic;

public abstract class PlantManager
{
    protected Atmosphere atmosphere;
    protected Terrain terrain;
    protected List<IntVector3> blocks;

    protected float LIFECYCLE_TICK_TIME = 1f;
    protected double SPREAD_CHANCE;
    protected IDictionary<Gas, float> GAS_DELTAS;

    static protected Random randGen = new Random();

    public PlantManager(Plants plants)
    {
        this.atmosphere = plants.atmosphere;
        this.terrain = plants.terrain;
        blocks = new List<IntVector3>();
    }

    abstract protected bool Valid(IntVector3 blockPos);
    abstract public bool PlantOn(IntVector3 blockPos);

    abstract public void LifeCycle(float delta);
    abstract protected void Spread();

    public void AdjustGases(float delta)
    {
        foreach (KeyValuePair<Gas, float> entry in GAS_DELTAS)
            atmosphere.ChangeGasAmt(entry.Key, entry.Value*blocks.Count);
    }
}
