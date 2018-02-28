using Godot;
using System;

public class AnimalPreset
{
    public AnimalBehaviourComponent.Sex sex;
    public PackedScene scene;
    public AnimalBehaviourComponent.Diet diet;
    public int foodChainLevel;
    public int breedability; //0 to 100
    public string presetName;
    public float oxygenConsumption;
    public float co2Production;

    public AnimalPreset(AnimalBehaviourComponent.Sex sex, PackedScene scene, AnimalBehaviourComponent.Diet diet, int foodChainLevel, int breedability, string presetName,
        float oxygenConsumption = 1.0f, float co2Production = 1.0f)
    {
        this.sex = sex;
        this.scene = scene;
        this.diet = diet;
        this.foodChainLevel = foodChainLevel;
        this.breedability = breedability;
        this.presetName = presetName;
        this.oxygenConsumption = oxygenConsumption;
        this.co2Production = co2Production;
    }
}
