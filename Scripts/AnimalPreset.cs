using Godot;
using System;

public class AnimalPreset
{
    public AnimalBehaviourComponent.Sex sex;
    public PackedScene scene;
    public AnimalBehaviourComponent.Diet diet;
    public int foodChainLevel;

    public AnimalPreset(AnimalBehaviourComponent.Sex sex, PackedScene scene, AnimalBehaviourComponent.Diet diet, int foodChainLevel)
    {
        this.sex = sex;
        this.scene = scene;
        this.diet = diet;
        this.foodChainLevel = foodChainLevel;
    }
}
