using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Bacteria
{
    private static readonly float DEFAULT_OPTIMAL_AMT = 1;
    private static readonly float DEFAULT_GROWTH_RATE = 0.002f;

    public float OptimalAmount { get; private set; }
    public float Amount { get; set; }
    public float GrowthRate { get; private set; }

    public Bacteria(float optimal) : this(0, DEFAULT_OPTIMAL_AMT, DEFAULT_GROWTH_RATE) {}

    public Bacteria(float amt, float optimal) : this(0, optimal, DEFAULT_GROWTH_RATE) { }

    public Bacteria(float amt, float optimal, float growthRate)
    {
        Amount = amt;
        OptimalAmount = optimal;
        GrowthRate = growthRate;
    }

    public void AddCapacity(float additionalCapacity)
    {
        OptimalAmount += additionalCapacity;
    }
}