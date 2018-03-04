using System;
using Godot;
using System.Collections.Generic;
using System.Linq;
using static ItemID;

public class ItemStorage
{
    private static Dictionary<byte, Item> blockItems = new Dictionary<byte, Item>();

    public static readonly IDictionary<ItemID, Item> items = new Dictionary<ItemID, Item>();

    private static readonly IDictionary<string, Texture> texDict;

    static ItemStorage()
    {
        // so far that's tradeoff between number of lines needed to add new item type
        // and number of lines needed to add new item of existing type
        // this version is left here with hope of further refactoring 
        // (ideally up to the point where item data is loaded from files)
        // TODO :: still ugly, fix (somehow)
        IList<string> texFileNames = new List<string>
        {
            "itemRedRock.png",
            "itemIce.png",
            "itemChocolate.png",
            "itemCake.png",
            "itemStone.png",
            "itemWater.png",
            "itemBacteriaFossil.png",
            "itemGrassFossil.png",
            "itemTreeFossil.png",
            "itemAnimalFossil.png",
            "itemOxygenBacteriaVial.png",
            "itemNitrogenBacteriaVial.png",
            "itemCarbonDioxideBacteriaVial.png",
            "itemGrass.png",
            "itemTree.png",
            "itemFrogEgg.png",
            "itemEgg.png",
            "itemBigEgg.png",
            "itemMeat.png"
        };

        // arbitrary choice: maps file name (without file extension) to texture
        texDict = texFileNames.ToDictionary(k => k.Split('.')[0],
            k => ResourceLoader.Load(Game.ITEM_TEXTURE_PATH + k) as Texture);

        // arbitrary choice: blocks are stackable
        var itemBlockInits = new List<Tuple<ItemID, string, string, byte>>
        {
            Tuple.Create(RED_ROCK, "Red Rock", "itemRedRock", Game.GetBlockId<RedRock>()),
            Tuple.Create(STONE_BLOCK, "Stone", "itemStone", Game.GetBlockId<Stone>()),
            Tuple.Create(ICE, "Ice", "itemIce", Game.GetBlockId<IceBlock>()),
            Tuple.Create(OXYGEN_BACTERIA_FOSSIL, "O2 Bacteria Fossil", "itemBacteriaFossil", Game.GetBlockId<OxygenBacteriaFossilBlock>()),
            Tuple.Create(NITROGEN_BACTERIA_FOSSIL, "N2 Bacteria Fossil", "itemBacteriaFossil", Game.GetBlockId<NitrogenBacteriaFossilBlock>()),
            Tuple.Create(CARBON_DIOXIDE_BACTERIA_FOSSIL, "CO2 Bacteria Fossil", "itemBacteriaFossil", Game.GetBlockId<CarbonDioxideBacteriaFossilBlock>()),
            Tuple.Create(GRASS_FOSSIL, "Grass Fossil", "itemGrassFossil", Game.GetBlockId<GrassFossilBlock>()),
            Tuple.Create(TREE_FOSSIL, "Tree Fossil", "itemTreeFossil", Game.GetBlockId<TreeFossilBlock>()),
            Tuple.Create(FROG_FOSSIL, "Frog Fossil", "itemAnimalFossil", Game.GetBlockId<FrogFossilBlock>()),
            Tuple.Create(REGULAR_ANIMAL_FOSSIL, "Animal Fossil", "itemAnimalFossil", Game.GetBlockId<RegularAnimalFossilBlock>()),
            Tuple.Create(BIG_ANIMAL_FOSSIL, "Giant Fossil", "itemAnimalFossil", Game.GetBlockId<BigAnimalFossilBlock>())
        };

        // arbitrary choice: consumables are not stackable
        var itemConsumableInits = new List<Tuple<ItemID, string, string, Player.Stats, float>>
        {
            Tuple.Create(CHOCOLATE, "Chocolate", "itemChocolate",Player.Stats.FOOD,0.2f),
            Tuple.Create(CAKE, "Cake", "itemCake",Player.Stats.FOOD,0.2f),
            Tuple.Create(WATER, "Water", "itemWater",Player.Stats.WATER,0.2f),
            Tuple.Create(MEAT, "Meat", "itemMeat", Player.Stats.FOOD, 0.4f)
        };

        // arbitrary choice: vials are stackable
        var itemVialInits = new List<Tuple<ItemID, string, string>>
        {
            Tuple.Create(OXYGEN_BACTERIA_VIAL, "O2 Bacteria", "itemOxygenBacteriaVial"),
            Tuple.Create(NITROGEN_BACTERIA_VIAL, "N2 Bacteria", "itemNitrogenBacteriaVial"),
            Tuple.Create(CARBON_DIOXIDE_BACTERIA_VIAL, "CO2 Bacteria", "itemCarbonDioxideBacteriaVial")
        };
        
        // arbitrary choice: plants are stackable
        var plantInits = new List<Tuple<ItemID, string, string, PlantType, byte, ItemID?>>
        {
            Tuple.Create(GRASS, "Grass", "itemGrass", PlantType.GRASS, Game.GetBlockId<GrassBlock>(), (ItemID?)RED_ROCK),
            Tuple.Create(TREE, "Tree", "itemTree", PlantType.TREE, Game.GetBlockId<TreeBlock>(),  (ItemID?)null /* TODO: change to tree */)
        };
        
        // arbitrary choice: eggs are not stackable
        var eggInits = new List<Tuple<ItemID, string, string, string>>
        {
            Tuple.Create(FROG_EGG, "Frog egg", "itemFrogEgg", "frog"),
            Tuple.Create(REGULAR_EGG, "Animal egg", "itemEgg", "animal0"),
            Tuple.Create(BIG_EGG, "Big animal egg", "itemBigEgg", "big")
        };

        foreach (var data in itemBlockInits)
        {
            items[data.Item1] = new ItemBlock(data.Item1, data.Item2, GetTexture(data.Item3), data.Item4);
            items[data.Item1].Stackable = true;
        }
        foreach (var data in itemConsumableInits)
        {
            items[data.Item1] = new ItemConsumable(data.Item1, data.Item2, GetTexture(data.Item3), data.Item4, data.Item5);
        }
        foreach (var data in itemVialInits)
        {
            items[data.Item1] = new ItemBacteriaVial(data.Item1, data.Item2, GetTexture(data.Item3));
            items[data.Item1].Stackable = true;
        }
        foreach (var data in plantInits)
        {
            items[data.Item1] = new ItemPlant(data.Item1, data.Item2, GetTexture(data.Item3), data.Item4, data.Item5, 
                data.Item6.HasValue ? items[data.Item6.Value] : null);
            items[data.Item1].Stackable = true;
        }
        foreach (var data in eggInits)
        {
            items[data.Item1] = new ItemSpawnEgg(data.Item1, data.Item2, GetTexture(data.Item3), data.Item4);
            items[data.Item1].Stackable = true;
        }
    }

<<<<<<< HEAD
    private static Texture TREE_TEX = ResourceLoader.Load(Game.ITEM_TEXTURE_PATH + "itemTree.png") as Texture;
    public static Item tree = new ItemPlant((byte) ItemID.TREE, "Tree", TREE_TEX, PlantType.TREE, Game.GetBlockId<TreeBlock>(), null /* TODO: change to tree */).SetStackable(true);
=======
    private static Texture GetTexture(string name)
    {
        bool found = texDict.TryGetValue(name, out Texture tex);
        if (found)
        {
            return tex;
        }
        else
        {
            throw new ArgumentException("'" + name + "' is not a name of any known texture");
        }
    }
>>>>>>> a84c7dc6cca15a846594375e2f6a3f7f52852593

    public static void RegisterBlockItem(byte id, Item item)
    {
        blockItems[id] = item;
    }

    public static Item GetItemFromBlock(byte b)
    {
        return blockItems.ContainsKey(b) ? blockItems[b] : null;
    }
}
