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
            90,
            "animal0"
        );

        AnimalPreset female0 = new AnimalPreset(
            AnimalBehaviourComponent.Sex.Female,
            (PackedScene)ResourceLoader.Load("res://scenes/FemaleAnimal0.tscn"),
            AnimalBehaviourComponent.Diet.Carnivore,
            10,
            90,
            "animal0"
        );

        AnimalPreset maleFrog = new AnimalPreset(
            AnimalBehaviourComponent.Sex.Male,
            (PackedScene)ResourceLoader.Load("res://scenes/MaleFrog.tscn"),
            AnimalBehaviourComponent.Diet.Herbivore,
            1,
            90,
            "frog"
        );

        AnimalPreset femaleFrog = new AnimalPreset(
            AnimalBehaviourComponent.Sex.Female,
            (PackedScene)ResourceLoader.Load("res://scenes/FemaleFrog.tscn"),
            AnimalBehaviourComponent.Diet.Herbivore,
            1,
            90,
            "frog"
        );

        AnimalPreset maleBig = new AnimalPreset(
            AnimalBehaviourComponent.Sex.Male,
            (PackedScene)ResourceLoader.Load("res://scenes/MaleBig.tscn"),
            AnimalBehaviourComponent.Diet.Omnivore,
            100,
            20,
            "big"
        );

        AnimalPreset femaleBig = new AnimalPreset(
            AnimalBehaviourComponent.Sex.Female,
            (PackedScene)ResourceLoader.Load("res://scenes/FemaleBig.tscn"),
            AnimalBehaviourComponent.Diet.Omnivore,
            100,
            20,
            "big"
        );

        presets.Add(male0);
        presets.Add(female0);
        presets.Add(maleFrog);
        presets.Add(femaleFrog);
        presets.Add(maleBig);
        presets.Add(femaleBig);

        physicsScript = (Script)ResourceLoader.Load("res://scripts/PhysicsComponent.cs");
        baseBehaviourScript = (Script)ResourceLoader.Load("res://scripts/AnimalBehaviourComponent.cs");

        SpawnAnimal("animal0",AnimalBehaviourComponent.Sex.Male,new Vector3(0.0f,50.0f,4.0f));
        SpawnAnimal("animal0",AnimalBehaviourComponent.Sex.Female,new Vector3(0.0f, 50.0f, 20.0f));

        SpawnAnimal("animal0", AnimalBehaviourComponent.Sex.Male, new Vector3(10.0f, 50.0f, 0.0f));
        SpawnAnimal("animal0", AnimalBehaviourComponent.Sex.Female, new Vector3(-10.0f, 50.0f, 0.0f));



        SpawnAnimal("frog", AnimalBehaviourComponent.Sex.Male, new Vector3(0.0f, 60.0f, 20.0f));
        SpawnAnimal("frog", AnimalBehaviourComponent.Sex.Female, new Vector3(0.0f, 50.0f, 4.0f));

        SpawnAnimal("frog", AnimalBehaviourComponent.Sex.Female, new Vector3(-5.0f, 50.0f, 20.0f));
        SpawnAnimal("frog", AnimalBehaviourComponent.Sex.Male, new Vector3(5.0f, 50.0f, 20.0f));

        SpawnAnimal("frog", AnimalBehaviourComponent.Sex.Female, new Vector3(0.0f, 50.0f, 25.0f));
        SpawnAnimal("frog", AnimalBehaviourComponent.Sex.Male, new Vector3(0.0f, 60.0f, 15.0f));

        SpawnAnimal("frog", AnimalBehaviourComponent.Sex.Female, new Vector3(-5.0f, 50.0f, 20.0f));
        SpawnAnimal("frog", AnimalBehaviourComponent.Sex.Male, new Vector3(5.0f, 50.0f, 4.0f));

        SpawnAnimal("frog", AnimalBehaviourComponent.Sex.Female, new Vector3(0.0f, 50.0f, 25.0f));
        SpawnAnimal("frog", AnimalBehaviourComponent.Sex.Male, new Vector3(0.0f, 60.0f, 4.0f));


        SpawnAnimal("frog", AnimalBehaviourComponent.Sex.Male, new Vector3(0.0f, 65.0f, 20.0f));
        SpawnAnimal("frog", AnimalBehaviourComponent.Sex.Female, new Vector3(0.0f, 55.0f, 4.0f));

        SpawnAnimal("frog", AnimalBehaviourComponent.Sex.Female, new Vector3(-5.0f, 55.0f, 20.0f));
        SpawnAnimal("frog", AnimalBehaviourComponent.Sex.Male, new Vector3(5.0f, 55.0f, 20.0f));

        SpawnAnimal("frog", AnimalBehaviourComponent.Sex.Female, new Vector3(0.0f, 55.0f, 25.0f));
        SpawnAnimal("frog", AnimalBehaviourComponent.Sex.Male, new Vector3(0.0f, 65.0f, 15.0f));

        SpawnAnimal("frog", AnimalBehaviourComponent.Sex.Female, new Vector3(-5.0f, 55.0f, 20.0f));
        SpawnAnimal("frog", AnimalBehaviourComponent.Sex.Male, new Vector3(5.0f, 55.0f, 4.0f));

        SpawnAnimal("frog", AnimalBehaviourComponent.Sex.Female, new Vector3(0.0f, 55.0f, 25.0f));
        SpawnAnimal("frog", AnimalBehaviourComponent.Sex.Male, new Vector3(0.0f, 65.0f, 4.0f));

        SpawnAnimal("big", AnimalBehaviourComponent.Sex.Male, new Vector3(0.0f, 50.0f, 40.0f));
        SpawnAnimal("big", AnimalBehaviourComponent.Sex.Female, new Vector3(40.0f, 50.0f, 0.0f));
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

        //TEMP: No food for frogs yet, so make sure they can breed immediately.
        if (presetName == "frog")
        {
            behaviourComponent.Set("satiated", 100);
        }
    }

//    public override void _Process(float delta)
//    {
//        // Called every frame. Delta is time since last frame.
//        // Update game logic here.
//        
//    }
}
