using System;
using Godot;
using System.Collections.Generic;

public class ItemStorage
{
    public enum ItemID : byte {
        RED_ROCK, STONE_BLOCK,
        OXYGEN_BACTERIA_FOSSIL, NITROGEN_BACTERIA_FOSSIL, CARBON_DIOXIDE_BACTERIA_FOSSIL,
        GRASS_FOSSIL, TREE_FOSSIL, ANIMAL_FOSSIL,
        OXYGEN_BACTERIA_VIAL, NITROGEN_BACTERIA_VIAL, CARBON_DIOXIDE_BACTERIA_VIAL,
        GRASS, TREE,
        CHOCOLATE, CAKE, WATER
    }
  
    private static Dictionary<byte, Item> blockItems = new Dictionary<byte, Item>();

    private static Texture RED_ROCK_TEX = ResourceLoader.Load(Game.ITEM_TEXTURE_PATH + "itemRedRock.png") as Texture;
    public static Item redRock = new ItemBlock((byte)ItemID.RED_ROCK, "Red Rock", RED_ROCK_TEX, Game.GetBlockId<RedRock>()).SetStackable(true);

    private static Texture CHOCOLATE_TEX = ResourceLoader.Load(Game.ITEM_TEXTURE_PATH + "itemChocolate.png") as Texture;
    public static Item chocolate = new ItemFood((byte)ItemID.CHOCOLATE, "Chocolate", CHOCOLATE_TEX, 0.2f);

    private static Texture CAKE_TEX = ResourceLoader.Load(Game.ITEM_TEXTURE_PATH + "itemCake.png") as Texture;
    public static Item cake = new ItemFood((byte)ItemID.CAKE, "Cake", CAKE_TEX, 0.5f);

    private static Texture STONE_BLOCK_TEX = ResourceLoader.Load(Game.ITEM_TEXTURE_PATH + "itemStone.png") as Texture;
    public static Item stone = new ItemBlock((byte)ItemID.STONE_BLOCK, "Stone", STONE_BLOCK_TEX, Game.GetBlockId<Stone>()).SetStackable(true);

    private static Texture WATER_TEX = ResourceLoader.Load(Game.ITEM_TEXTURE_PATH + "itemWater.png") as Texture;
    public static Item water = new ItemDrink((byte)ItemID.WATER, "Water", WATER_TEX, 0.4f);

    private static Texture BACTERIA_FOSSIL_TEX = ResourceLoader.Load(Game.ITEM_TEXTURE_PATH + "itemBacteriaFossil.png") as Texture;
    public static Item oxygenBacteriaFossil = 
        new ItemBlock((byte)ItemID.OXYGEN_BACTERIA_FOSSIL, "OxygenBacteriaFossil", BACTERIA_FOSSIL_TEX, Game.GetBlockId<OxygenBacteriaFossilBlock>()).SetStackable(true);

    public static Item nitrogenBacteriaFossil = 
        new ItemBlock((byte)ItemID.NITROGEN_BACTERIA_FOSSIL, "NitrogenBacteriaFossil", BACTERIA_FOSSIL_TEX, Game.GetBlockId<NitrogenBacteriaFossilBlock>()).SetStackable(true);

    public static Item carbonDioxideBacteriaFossil = 
        new ItemBlock((byte)ItemID.CARBON_DIOXIDE_BACTERIA_FOSSIL, "CarbonDioxideBacteriaFossil", BACTERIA_FOSSIL_TEX, Game.GetBlockId<CarbonDioxideBacteriaFossilBlock>()).SetStackable(true);
    
    private static Texture GRASS_FOSSIL_TEX = ResourceLoader.Load(Game.ITEM_TEXTURE_PATH + "itemGrassFossil.png") as Texture;
    public static Item grassFossil =
        new ItemBlock((byte)ItemID.GRASS_FOSSIL, "GrassFossil", GRASS_FOSSIL_TEX, Game.GetBlockId<GrassFossilBlock>()).SetStackable(true);

    private static Texture TREE_FOSSIL_TEX = ResourceLoader.Load(Game.ITEM_TEXTURE_PATH + "itemTreeFossil.png") as Texture;
    public static Item treeFossil =
        new ItemBlock((byte)ItemID.TREE_FOSSIL, "TreeFossil", TREE_FOSSIL_TEX, Game.GetBlockId<TreeFossilBlock>()).SetStackable(true);

    private static Texture ANIMAL_FOSSIL_TEX = ResourceLoader.Load(Game.ITEM_TEXTURE_PATH + "itemAnimalFossil.png") as Texture;
    public static Item animalFossil =
        new ItemBlock((byte)ItemID.ANIMAL_FOSSIL, "AnimalFossil", ANIMAL_FOSSIL_TEX, Game.GetBlockId<AnimalFossilBlock>()).SetStackable(true);

    private static Texture OXYGEN_BACTERIA_VIAL_TEX = ResourceLoader.Load(Game.ITEM_TEXTURE_PATH + "itemOxygenBacteriaVial.png") as Texture;
    public static Item oxygenBacteriaVial = 
        new ItemBacteriaVial((byte)ItemID.OXYGEN_BACTERIA_VIAL, "OxygenBacteriaVial", OXYGEN_BACTERIA_VIAL_TEX);

    private static Texture NITROGEN_BACTERIA_VIAL_TEX = ResourceLoader.Load(Game.ITEM_TEXTURE_PATH + "itemNitrogenBacteriaVial.png") as Texture;
    public static Item nitrogenBacteriaVial = 
        new ItemBacteriaVial((byte)ItemID.NITROGEN_BACTERIA_VIAL, "NitrogenBacteriaVial", NITROGEN_BACTERIA_VIAL_TEX);
    
    private static Texture CARBON_DIOXIDE_BACTERIA_VIAL_TEX = ResourceLoader.Load(Game.ITEM_TEXTURE_PATH + "itemCarbonDioxideBacteriaVial.png") as Texture;
    public static Item carbonDioxideBacteriaVial = 
        new ItemBacteriaVial((byte)ItemID.CARBON_DIOXIDE_BACTERIA_VIAL, "CarbonDioxideBacteriaVial", CARBON_DIOXIDE_BACTERIA_VIAL_TEX);

    private static Texture GRASS_TEX = ResourceLoader.Load(Game.ITEM_TEXTURE_PATH + "itemGrass.png") as Texture;
    public static Item grass = new ItemPlant((byte) ItemID.GRASS, "Grass", GRASS_TEX, PlantType.GRASS, Game.GetBlockId<GrassBlock>(), redRock).SetStackable(true);

    private static Texture TREE_TEX = ResourceLoader.Load(Game.ITEM_TEXTURE_PATH + "itemTree.png") as Texture;
    public static Item tree = new ItemPlant((byte) ItemID.TREE, "Tree", TREE_TEX, PlantType.TREE, Game.GetBlockId<TreeBlock>(), redRock /* TODO: change to tree */).SetStackable(true);

    public static void RegisterBlockItem(byte id, Item item)
    {
        blockItems[id] = item;
    }

    public static Item GetItemFromBlock(byte b)
    {
        return blockItems.ContainsKey(b) ? blockItems[b] : null;
    }
}