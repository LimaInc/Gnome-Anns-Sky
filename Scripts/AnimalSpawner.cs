using Godot;
using System;
using System.Collections.Generic;

public class AnimalSpawner : Node
{
    private List<AnimalPreset> presets;
    private Script physicsScript;
    private Script baseBehaviourScript;

    public override void _Ready()
    {
        presets = new List<AnimalPreset>();

        // For now, hardcode presets
        AnimalPreset male0 = new AnimalPreset(
            AnimalBehaviourComponent.Sex.Male,
            (PackedScene)ResourceLoader.Load("res://scenes/MaleAnimal0.tscn"),
            AnimalBehaviourComponent.Diet.Carnivore,
            10,
            80,
            "animal0"
        );

        AnimalPreset female0 = new AnimalPreset(
            AnimalBehaviourComponent.Sex.Female,
            (PackedScene)ResourceLoader.Load("res://scenes/FemaleAnimal0.tscn"),
            AnimalBehaviourComponent.Diet.Carnivore,
            10,
            80,
            "animal0"
        );

        presets.Add(male0);
        presets.Add(female0);

        physicsScript = (Script)ResourceLoader.Load("res://scripts/PhysicsComponent.cs");
        baseBehaviourScript = (Script)ResourceLoader.Load("res://scripts/AnimalBehaviourComponent.cs");

        SpawnAnimal("animal0",AnimalBehaviourComponent.Sex.Male,new Vector3(0.0f,50.0f,4.0f));
        SpawnAnimal("animal0",AnimalBehaviourComponent.Sex.Female,new Vector3(0.0f, 50.0f, 20.0f));
    }

    public void SpawnAnimal(string presetName,AnimalBehaviourComponent.Sex sex,Vector3 position)
    {
        // Choose preset
        //for now, choose base preset
        AnimalPreset preset = null;
        foreach(AnimalPreset p in presets)
        {
            if(p.presetName.Equals(presetName) && p.sex == sex)
            {
                preset = p;
                break;
            }
        }

        // Use preset to generate animal
        PackedScene chosenScene = preset.scene;

        KinematicBody kb = (KinematicBody)chosenScene.Instance();

        var physicsComponent = new Node();
        physicsComponent.SetScript(physicsScript);

        var behaviourComponent = new Node();
        behaviourComponent.SetName("BehaviourComponent");
        behaviourComponent.SetScript(baseBehaviourScript);

        //TODO: make this an array of auto-settable properties
        behaviourComponent.Set("sex", (int)preset.sex);
        behaviourComponent.Set("diet", (int)preset.diet);
        behaviourComponent.Set("foodChainLevel", preset.foodChainLevel);
        behaviourComponent.Set("breedability", preset.breedability);
        behaviourComponent.Set("presetName", preset.presetName);

        kb.AddChild(physicsComponent);
        kb.AddChild(behaviourComponent);

        kb.SetTranslation(position);

        AddChild(kb);        
    }

//    public override void _Process(float delta)
//    {
//        // Called every frame. Delta is time since last frame.
//        // Update game logic here.
//        
//    }
}
