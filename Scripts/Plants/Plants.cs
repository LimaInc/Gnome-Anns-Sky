using System;
using System.Collections.Generic;
using Godot;

public class Plants : Node
{
    private Terrain terrain;

    private Dictionary<PlantType, PlantManager> plantManagers;

    public override void _Ready()
    {
        terrain = GetNode(Game.TERRAIN_PATH) as Terrain;

        plantManagers = new Dictionary<PlantType, PlantManager> {
            [PlantType.GRASS] = new GrassManager(terrain),
            [PlantType.TREE] = new TreeManager(terrain)
        };
    }

    public bool Plant(ItemPlant plantItem, IntVector3 blockPos)
    {
        if (plantManagers.ContainsKey(plantItem.PlantType))
            return plantManagers[plantItem.PlantType].PlantOn(blockPos);
        return false;
    }

    public override void _Process(float delta)
    {
        foreach (PlantManager plantManager in plantManagers.Values)
            plantManager.Spread(delta);
        // Called every frame. Delta is time since last frame.
        // Update game logic here.
    }
}
