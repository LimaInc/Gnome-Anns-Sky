using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Bacteria
{
    public static readonly IDictionary<BacteriumType, Gas> defaultProduction = new Dictionary<BacteriumType, Gas>
    {
        [BacteriumType.OXYGEN] = Gas.OXYGEN,
        [BacteriumType.NITROGEN] = Gas.NITROGEN,
        [BacteriumType.CARBON_DIOXIDE] = Gas.CARBON_DIOXIDE
    };

    private const float DEFAULT_OPTIMAL_AMT = 1;
    private const float DEFAULT_GROWTH_RATE = 0.002f;
    private const float DEFAULT_PRODUCTION_RATE = 0.2f;

    public BacteriumType Type { get; }
    public Gas GasType { get; }
    public float OptimalAmount { get; private set; }
    public float Amount { get; set; }
    public float GrowthRate { get; private set; }
    public float ProductionRate { get; private set; }

    public Bacteria(BacteriumType type, float amt = 0, float optimal = DEFAULT_OPTIMAL_AMT, 
                        float growthRate = DEFAULT_GROWTH_RATE, float productionRate = DEFAULT_PRODUCTION_RATE)
        : this(type, defaultProduction[type], amt, optimal, growthRate, productionRate) { }

    public Bacteria(BacteriumType type, Gas g, float amt, float optimal, float growthRate, float productionRate)
    {
        Type = type;
        GasType = g;
        Amount = amt;
        OptimalAmount = optimal;
        GrowthRate = growthRate;
        ProductionRate = productionRate;
    }

    public void AddCapacity(float additionalCapacity)
    {
        OptimalAmount += additionalCapacity;
    }

    public void AddAmt(float amt)
    {
        Amount += amt;
    }
}