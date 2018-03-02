using System;
using System.Collections.Generic;
using Godot;

public class Plants : Node
{
    public Atmosphere atmosphere;
    public Terrain terrain;

    private Dictionary<PlantType, PlantManager> plantManagers;

    public override void _Ready()
    {
        atmosphere = GetNode(Game.ATMOSPHERE_PATH) as Atmosphere;
        terrain = GetNode(Game.TERRAIN_PATH) as Terrain;

        plantManagers = new Dictionary<PlantType, PlantManager> {
            [PlantType.GRASS] = new GrassManager(this),
            [PlantType.TREE] = new TreeManager(this)
        };
    }

    public bool Plant(ItemPlant plantItem, IntVector3 blockPos)
    {
        if (plantManagers.ContainsKey(plantItem.PType))
            return plantManagers[plantItem.PType].PlantOn(blockPos);
        return false;
    }

    public override void _PhysicsProcess(float delta)
    {
        // TODO: fix plants so that there is no reasonable limit to speedup
        delta *= Math.Min(Game.SPEED, Game.PLANT_MAX_SPEED);
        foreach (PlantManager plantManager in plantManagers.Values)
        {
            plantManager.LifeCycle(delta);
            plantManager.AdjustGases(delta);
        }
    }
}
