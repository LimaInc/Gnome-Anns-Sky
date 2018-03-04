using Godot;
using System;
using System.Collections.Generic;

public class AnimalSpawner : Node
{
    private List<AnimalPreset> presets;
    Script entityScript = (Script)ResourceLoader.Load("res://Scripts/Animals/Entity.cs");

    private void HollisticDemo()
    {
       // SpawnAnimal("animal0", AnimalBehaviourComponent.AnimalSex.Male, new Vector3(0.0f, 50.0f, 4.0f));
        //SpawnAnimal("animal0", AnimalBehaviourComponent.AnimalSex.Female, new Vector3(0.0f, 50.0f, 20.0f));

     //   SpawnAnimal("animal0", AnimalBehaviourComponent.AnimalSex.Male, new Vector3(10.0f, 50.0f, 0.0f));
       // SpawnAnimal("animal0", AnimalBehaviourComponent.AnimalSex.Female, new Vector3(-10.0f, 50.0f, 0.0f));

        /*SpawnAnimal("frog", AnimalBehaviourComponent.AnimalSex.Male, new Vector3(0.0f, 60.0f, 20.0f));
        SpawnAnimal("frog", AnimalBehaviourComponent.AnimalSex.Female, new Vector3(0.0f, 50.0f, 4.0f));

        SpawnAnimal("frog", AnimalBehaviourComponent.AnimalSex.Female, new Vector3(-5.0f, 50.0f, 20.0f));
        SpawnAnimal("frog", AnimalBehaviourComponent.AnimalSex.Male, new Vector3(5.0f, 50.0f, 20.0f));

        SpawnAnimal("frog", AnimalBehaviourComponent.AnimalSex.Female, new Vector3(0.0f, 50.0f, 25.0f));
        SpawnAnimal("frog", AnimalBehaviourComponent.AnimalSex.Male, new Vector3(0.0f, 60.0f, 15.0f));

        SpawnAnimal("frog", AnimalBehaviourComponent.AnimalSex.Female, new Vector3(-5.0f, 50.0f, 20.0f));
        SpawnAnimal("frog", AnimalBehaviourComponent.AnimalSex.Male, new Vector3(5.0f, 50.0f, 4.0f));

        SpawnAnimal("frog", AnimalBehaviourComponent.AnimalSex.Female, new Vector3(0.0f, 50.0f, 25.0f));
        SpawnAnimal("frog", AnimalBehaviourComponent.AnimalSex.Male, new Vector3(0.0f, 60.0f, 4.0f));


        SpawnAnimal("frog", AnimalBehaviourComponent.AnimalSex.Male, new Vector3(0.0f, 65.0f, 20.0f));
        SpawnAnimal("frog", AnimalBehaviourComponent.AnimalSex.Female, new Vector3(0.0f, 55.0f, 4.0f));

        SpawnAnimal("frog", AnimalBehaviourComponent.AnimalSex.Female, new Vector3(-5.0f, 55.0f, 20.0f));
        SpawnAnimal("frog", AnimalBehaviourComponent.AnimalSex.Male, new Vector3(5.0f, 55.0f, 20.0f));*/

        SpawnAnimal("frog", AnimalBehaviourComponent.AnimalSex.Female, new Vector3(0.0f, 55.0f, 25.0f));
        SpawnAnimal("frog", AnimalBehaviourComponent.AnimalSex.Male, new Vector3(0.0f, 65.0f, 15.0f));

        SpawnAnimal("frog", AnimalBehaviourComponent.AnimalSex.Female, new Vector3(-5.0f, 55.0f, 20.0f));
        SpawnAnimal("frog", AnimalBehaviourComponent.AnimalSex.Male, new Vector3(5.0f, 55.0f, 4.0f));

        SpawnAnimal("frog", AnimalBehaviourComponent.AnimalSex.Female, new Vector3(0.0f, 55.0f, 25.0f));
        SpawnAnimal("frog", AnimalBehaviourComponent.AnimalSex.Male, new Vector3(0.0f, 65.0f, 4.0f));

    //    SpawnAnimal("big", AnimalBehaviourComponent.AnimalSex.Male, new Vector3(0.0f, 50.0f, 40.0f));
      //  SpawnAnimal("big", AnimalBehaviourComponent.AnimalSex.Female, new Vector3(40.0f, 50.0f, 0.0f));

    }

    private void BreedingDemo()
    {
        SpawnAnimal("animal0BD", AnimalBehaviourComponent.AnimalSex.Male, new Vector3(5.0f, 50.0f, 15.0f));
        SpawnAnimal("animal0BD", AnimalBehaviourComponent.AnimalSex.Female, new Vector3(-5.0f, 50.0f, 15.0f));

        // SpawnAnimal("frogBD", AnimalBehaviourComponent.AnimalSex.Male, new Vector3(0.0f, 60.0f, 0.0f));
        // SpawnAnimal("frogBD", AnimalBehaviourComponent.AnimalSex.Female, new Vector3(2.0f, 50.0f, 0.0f));

        // SpawnAnimal("frogBD", AnimalBehaviourComponent.AnimalSex.Female, new Vector3(-2.0f, 50.0f, -2.0f));
        // SpawnAnimal("frogBD", AnimalBehaviourComponent.AnimalSex.Male, new Vector3(2.0f, 50.0f, -2.0f));
    }

    private void EatingDemo()
    {
        SpawnAnimal("big", AnimalBehaviourComponent.AnimalSex.Male, new Vector3(0.0f, 50.0f, 20.0f));
        SpawnAnimal("frog", AnimalBehaviourComponent.AnimalSex.Female, new Vector3(0.0f, 55.0f, 1.0f));
        SpawnAnimal("frog", AnimalBehaviourComponent.AnimalSex.Female, new Vector3(0.0f, 55.0f, 0.0f));
        SpawnAnimal("animal0", AnimalBehaviourComponent.AnimalSex.Male, new Vector3(0.0f, 50.0f, 10.0f));
    }


    public override void _Ready()
    {
        presets = new List<AnimalPreset>();

        // For now, hardcode presets
        AnimalPreset male0 = new AnimalPreset(
            AnimalBehaviourComponent.AnimalSex.Male,
            (PackedScene)ResourceLoader.Load("res://Scenes/MaleAnimal0.tscn"),
            AnimalBehaviourComponent.AnimalDiet.Carnivore,
            10,
            90,
            "animal0",
            2,
            10
        );

        AnimalPreset female0 = new AnimalPreset(
            AnimalBehaviourComponent.AnimalSex.Female,
            (PackedScene)ResourceLoader.Load("res://Scenes/FemaleAnimal0.tscn"),
            AnimalBehaviourComponent.AnimalDiet.Carnivore,
            10,
            90,
            "animal0",
            2,
            10
        );

        AnimalPreset maleFrog = new AnimalPreset(
            AnimalBehaviourComponent.AnimalSex.Male,
            (PackedScene)ResourceLoader.Load("res://Scenes/MaleFrog.tscn"),
            AnimalBehaviourComponent.AnimalDiet.Herbivore,
            1,
            90,
            "frog",
            1,
            5
        );

        AnimalPreset femaleFrog = new AnimalPreset(
            AnimalBehaviourComponent.AnimalSex.Female,
            (PackedScene)ResourceLoader.Load("res://Scenes/FemaleFrog.tscn"),
            AnimalBehaviourComponent.AnimalDiet.Herbivore,
            1,
            90,
            "frog",
            1,
            5
        );

        AnimalPreset maleBig = new AnimalPreset(
            AnimalBehaviourComponent.AnimalSex.Male,
            (PackedScene)ResourceLoader.Load("res://Scenes/MaleBig.tscn"),
            AnimalBehaviourComponent.AnimalDiet.Omnivore,
            100,
            70,
            "big",
            3,
            20
        );

        AnimalPreset femaleBig = new AnimalPreset(
            AnimalBehaviourComponent.AnimalSex.Female,
            (PackedScene)ResourceLoader.Load("res://Scenes/FemaleBig.tscn"),
            AnimalBehaviourComponent.AnimalDiet.Omnivore,
            100,
            70,
            "big",
            3,
            20
        );

        AnimalPreset maleFrogBD = new AnimalPreset(
            AnimalBehaviourComponent.AnimalSex.Male,
            (PackedScene)ResourceLoader.Load("res://Scenes/MaleFrog.tscn"),
            AnimalBehaviourComponent.AnimalDiet.Herbivore,
            1,
            80,
            "frogBD"
        );

        AnimalPreset femaleFrogBD = new AnimalPreset(
            AnimalBehaviourComponent.AnimalSex.Female,
            (PackedScene)ResourceLoader.Load("res://Scenes/FemaleFrog.tscn"),
            AnimalBehaviourComponent.AnimalDiet.Herbivore,
            1,
            80,
            "frogBD"
        );

        AnimalPreset male0BD = new AnimalPreset(
            AnimalBehaviourComponent.AnimalSex.Male,
            (PackedScene)ResourceLoader.Load("res://Scenes/MaleAnimal0.tscn"),
            AnimalBehaviourComponent.AnimalDiet.Carnivore,
            1,
            80,
            "animal0BD"
        );

        AnimalPreset female0BD = new AnimalPreset(
            AnimalBehaviourComponent.AnimalSex.Female,
            (PackedScene)ResourceLoader.Load("res://Scenes/FemaleAnimal0.tscn"),
            AnimalBehaviourComponent.AnimalDiet.Carnivore,
            1,
            80,
            "animal0BD"
        );


        presets.Add(male0);
        presets.Add(female0);
        presets.Add(maleFrog);
        presets.Add(femaleFrog);
        presets.Add(maleBig);
        presets.Add(femaleBig);

        presets.Add(male0BD);
        presets.Add(female0BD);
        presets.Add(maleFrogBD);
        presets.Add(femaleFrogBD);

        //Code for demos.
        /*
        BreedingDemo();
        EatingDemo(); */
        //HollisticDemo();
    }

    private float time;
    bool spawnedDemo = false;

    public override void _Process(float delta)
    {
        base._Process(delta);

        time += delta;

        if(time > 3.0f && !spawnedDemo)
        {
            //HollisticDemo();
            spawnedDemo = true;
        }
    }


    public void SpawnAnimal(string presetName, AnimalBehaviourComponent.AnimalSex sex, Vector3 position)
    {
        // Choose preset
        AnimalPreset preset = null;
        foreach (AnimalPreset p in presets)
        {
            if (p.presetName.Equals(presetName) && p.sex == sex)
            {
                preset = p;
                break;
            }
        }

        // Use preset to generate animal
        PackedScene chosenScene = preset.scene;

        KinematicBody kb = (KinematicBody)chosenScene.Instance();

        Entity entity = new Entity();
        kb.AddChild(entity);

        entity.SetName("Entity");

        AnimalBehaviourComponent behaviourComponent = new AnimalBehaviourComponent(entity, preset.sex, preset.diet, preset.foodChainLevel, preset.breedability,
                            preset.presetName, preset.oxygenConsumption, preset.co2Production, preset.foodDrop, preset.birthDrop);
        PhysicsComponent physicsComponent = new PhysicsComponent(entity);
        entity.AddComponent(behaviourComponent);
        entity.AddComponent(physicsComponent);

        kb.SetTranslation(position);

        AddChild(kb);
    }
}
