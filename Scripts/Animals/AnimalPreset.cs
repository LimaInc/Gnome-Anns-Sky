using Godot;
using System;

public class AnimalPreset
{
    public AnimalBehaviourComponent.AnimalSex sex;
    public PackedScene scene;
    public AnimalBehaviourComponent.AnimalDiet diet;
    public int foodChainLevel;
    public int breedability; //0 to 100
    public string presetName;
    public float oxygenConsumption;
    public float co2Production;

    public AnimalPreset(AnimalBehaviourComponent.AnimalSex sex, PackedScene scene, AnimalBehaviourComponent.AnimalDiet diet, int foodChainLevel, int breedability, string presetName,
        float oxygenConsumption = 0.0001f, float co2Production = 0.0001f)
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
