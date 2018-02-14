using Godot;
using System;
using System.Collections.Generic;

public class AnimalSpawner : Node
{
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";

    private List<AnimalPreset> presets;
    private Script physicsScript;
    private Script baseBehaviourScript;

    public override void _Ready()
    {
        presets = new List<AnimalPreset>();

        // For now, hardcore presets
        AnimalPreset male0 = new AnimalPreset(
            AnimalBehaviourComponent.Sex.Male,
            (PackedScene)ResourceLoader.Load("res://scenes/MaleLandAnimal0.tscn"),
            AnimalBehaviourComponent.Diet.Carnivore,
            10
        );

        AnimalPreset female0 = new AnimalPreset(
            AnimalBehaviourComponent.Sex.Female,
            (PackedScene)ResourceLoader.Load("res://scenes/FemaleLandAnimal0.tscn"),
            AnimalBehaviourComponent.Diet.Carnivore,
            9
        );

        presets.Add(male0);
        presets.Add(female0);

        physicsScript = (Script)ResourceLoader.Load("res://scripts/PhysicsComponent.cs");
        baseBehaviourScript = (Script)ResourceLoader.Load("res://scripts/AnimalBehaviourComponent.cs");

        SpawnAnimal(0,new Vector3(0.0f,50.0f,4.0f));
        SpawnAnimal(1,new Vector3(0.0f, 50.0f, 40.0f));
    }

    public void SpawnAnimal(int pn,Vector3 position)
    {
        // Choose preset
        //for now, choose base preset
        AnimalPreset preset = presets[pn];


        // Use preset to generate animal
        PackedScene chosenScene = preset.scene;

        Spatial spatial = (Spatial)chosenScene.Instance();
        KinematicBody kb = (KinematicBody) spatial.GetNode(new NodePath("KinematicBody"));

        var physicsComponent = new Node();
        physicsComponent.SetScript(physicsScript);

        var behaviourComponent = new Node();
        behaviourComponent.SetScript(baseBehaviourScript);
        behaviourComponent.Set("sex", (int)AnimalBehaviourComponent.Sex.Female);

        kb.AddChild(physicsComponent);
        kb.AddChild(behaviourComponent);

        spatial.AddChild(kb);
        spatial.SetTranslation(position);

        AddChild(spatial);        
    }

//    public override void _Process(float delta)
//    {
//        // Called every frame. Delta is time since last frame.
//        // Update game logic here.
//        
//    }
}
