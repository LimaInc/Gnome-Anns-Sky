using System;
using System.Collections.Generic;
using Godot;

public abstract class PlantManager : Node
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

    abstract protected bool CanGrowOn(IntVector3 blockPos);
    abstract public bool PlantOn(IntVector3 blockPos);

    abstract public void LifeCycle(float delta);
    abstract protected void Spread();

    abstract public void HandleBlockChange(byte oldId, byte newId, IntVector3 pos);

    public void AdjustGases(float delta)
    {
        foreach (KeyValuePair<Gas, float> entry in gasDeltas)
            atmosphere.ChangeGasAmt(entry.Key, entry.Value * plantBlocks.Count);
    }

    public override void _PhysicsProcess(float delta)
    {
        delta *= Mathf.Min(Game.SPEED, Game.PLANT_MAX_SPEED);
        LifeCycle(delta);
        AdjustGases(delta);
    }
}
