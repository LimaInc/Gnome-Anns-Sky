using System;
using System.Collections.Generic;
using Godot;

public class Plants : Node
{
    private static readonly byte GRASS_ID = Game.GetBlockId<GrassBlock>();
    private static readonly byte RED_ROCK_ID = Game.GetBlockId<RedRock>();

    public Atmosphere atmosphere;
    public Terrain terrain;

    private Dictionary<PlantType, PlantManager> plantManagers;

    public override void _Ready()
    {
        atmosphere = GetNode(Game.ATMOSPHERE_PATH) as Atmosphere;
        terrain = GetNode(Game.TERRAIN_PATH) as Terrain;

        plantManagers = new Dictionary<PlantType, PlantManager> {
            [PlantType.GRASS] = new GrassManager(this),
            [PlantType.TREE] = new TreeManager(this),
            [PlantType.WHEAT] = new WheatManager(this)
        };
        foreach (PlantManager pm in plantManagers.Values)
        {
            AddChild(pm);
        }
    }

    public bool Plant(ItemPlant plantItem, IntVector3 blockPos)
    {
        if (plantManagers.ContainsKey(plantItem.PType))
        {
            return plantManagers[plantItem.PType].PlantOn(blockPos);
        }
        return false;
    }

    public PlantManager this[PlantType i]
    {
        get => plantManagers[i];
    }

    internal void HandleBlockChange(byte oldBlockId, byte newBlockId, IntVector3 blockPos)
    {
        foreach (PlantManager pm in plantManagers.Values)
        {
            pm.HandleBlockChange(oldBlockId, newBlockId, blockPos);
        }
    }
}
