using Godot;
using System;
using System.Collections.Generic;

public class AnimalSpawner : Node
{
    private List<AnimalPreset> presets;
    Script entityScript = (Script)ResourceLoader.Load("res://Scripts/Animals/Entity.cs");

    private void HollisticDemo()
    {
        SpawnAnimal("animal0", AnimalBehaviourComponent.Sex.Male, new Vector3(0.0f, 50.0f, 4.0f));
        SpawnAnimal("animal0", AnimalBehaviourComponent.Sex.Female, new Vector3(0.0f, 50.0f, 20.0f));

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

    private void BreedingDemo()
    {
        SpawnAnimal("animal0BD", AnimalBehaviourComponent.Sex.Male, new Vector3(5.0f, 50.0f, 0.0f));
        SpawnAnimal("animal0BD", AnimalBehaviourComponent.Sex.Female, new Vector3(-5.0f, 50.0f, 0.0f));

       // SpawnAnimal("frogBD", AnimalBehaviourComponent.Sex.Male, new Vector3(0.0f, 60.0f, 0.0f));
       // SpawnAnimal("frogBD", AnimalBehaviourComponent.Sex.Female, new Vector3(2.0f, 50.0f, 0.0f));

      //  SpawnAnimal("frogBD", AnimalBehaviourComponent.Sex.Female, new Vector3(-2.0f, 50.0f, -2.0f));
        //SpawnAnimal("frogBD", AnimalBehaviourComponent.Sex.Male, new Vector3(2.0f, 50.0f, -2.0f));
    }

    private void EatingDemo()
    {
        SpawnAnimal("big", AnimalBehaviourComponent.Sex.Male, new Vector3(0.0f, 50.0f, 20.0f));
        SpawnAnimal("frog", AnimalBehaviourComponent.Sex.Female, new Vector3(0.0f, 55.0f, 1.0f));
        SpawnAnimal("frog", AnimalBehaviourComponent.Sex.Female, new Vector3(0.0f, 55.0f, 0.0f));
        SpawnAnimal("animal0", AnimalBehaviourComponent.Sex.Male, new Vector3(0.0f, 50.0f, 10.0f));
    }

    

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

        AnimalPreset maleFrogBD = new AnimalPreset(
            AnimalBehaviourComponent.Sex.Male,
            (PackedScene)ResourceLoader.Load("res://scenes/MaleFrog.tscn"),
            AnimalBehaviourComponent.Diet.Herbivore,
            1,
            80,
            "frogBD"
        );

        AnimalPreset femaleFrogBD = new AnimalPreset(
            AnimalBehaviourComponent.Sex.Female,
            (PackedScene)ResourceLoader.Load("res://scenes/FemaleFrog.tscn"),
            AnimalBehaviourComponent.Diet.Herbivore,
            1,
            80,
            "frogBD"
        );

        AnimalPreset male0BD = new AnimalPreset(
            AnimalBehaviourComponent.Sex.Male,
            (PackedScene)ResourceLoader.Load("res://scenes/MaleAnimal0.tscn"),
            AnimalBehaviourComponent.Diet.Carnivore,
            1,
            80,
            "animal0BD"
        );

        AnimalPreset female0BD = new AnimalPreset(
            AnimalBehaviourComponent.Sex.Female,
            (PackedScene)ResourceLoader.Load("res://scenes/FemaleAnimal0.tscn"),
            AnimalBehaviourComponent.Diet.Carnivore,
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

   

        //BreedingDemo();
        //EatingDemo();
        HollisticDemo();
    }

    public void SpawnAnimal(string presetName,AnimalBehaviourComponent.Sex sex,Vector3 position)
    {
        // Choose preset
        // for now, choose base preset
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

        Entity entity = new Entity();
        kb.AddChild(entity);

        entity.SetName("Entity");

        AnimalBehaviourComponent behaviourComponent = new AnimalBehaviourComponent(entity, preset.sex, preset.diet, preset.foodChainLevel, preset.breedability, preset.presetName);
        PhysicsComponent physicsComponent = new PhysicsComponent(entity);
        entity.AddComponent(behaviourComponent);
        entity.AddComponent(physicsComponent);

        kb.SetTranslation(position);

        GD.Print("Adding object!");
        AddChild(kb);

        //TEMP: No food for frogs yet, so make sure they can breed immediately.
        if (presetName == "frog")
        {
            behaviourComponent.satiated = 100;
        }
    }
}
