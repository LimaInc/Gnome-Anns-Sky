using System;
using System.Collections.Generic;

public abstract class PlantManager
{
    protected Terrain terrain;
    protected List<IntVector3> blocks;

    protected float SPREAD_TIME = 1f;
    protected double SPREAD_CHANCE;

    static protected Random randGen = new Random();

    public PlantManager(Terrain terrain)
    {
        this.terrain = terrain;
        blocks = new List<IntVector3>();
    }

    abstract public bool PlantOn(IntVector3 blockPos);

    abstract public void Spread(float delta);
}
