using System;
using Godot;
using System.Collections.Generic;

public class ItemStorage
{
    public enum ItemID : byte {
        RED_ROCK, FOSSIL, STONE_BLOCK,
        OXYGEN_BACTERIA_FOSSIL, NITROGEN_BACTERIA_FOSSIL, CARBON_DIOXIDE_BACTERIA_FOSSIL,
        OXYGEN_BACTERIA_VIAL, NITROGEN_BACTERIA_VIAL, CARBON_DIOXIDE_BACTERIA_VIAL,
        CHOCOLATE, CAKE, WATER,
    }
  
    private static Dictionary<byte, Item> blockItems = new Dictionary<byte, Item>();

    private static Texture RED_ROCK_TEX = ResourceLoader.Load("res://Images/itemRedRock.png") as Texture;
    public static Item redRock = new ItemBlock(1, "Red Rock", RED_ROCK_TEX, Game.GetBlockId<RedRock>()).SetStackable(true);

    private static Texture FOSSIL_TEX = ResourceLoader.Load("res://Images/itemFossil.png") as Texture;
    public static Item fossil = new Item(2, "Fossil", FOSSIL_TEX, Item.Type.FOSSIL);

    private static Texture CHOCOLATE_TEX = ResourceLoader.Load("res://Images/itemChocolate.png") as Texture;
    public static Item chocolate = new ItemFood(3, "Chocolate", CHOCOLATE_TEX, 0.2f);

    private static Texture CAKE_TEX = ResourceLoader.Load("res://Images/itemCake.png") as Texture;
    public static Item cake = new ItemFood(4, "Cake", CAKE_TEX, 0.5f);

    private static Texture STONE_BLOCK_TEX = ResourceLoader.Load("res://Images/itemStone.png") as Texture;
    public static Item stone = new ItemBlock(5, "Stone", STONE_BLOCK_TEX, Game.GetBlockId<Stone>()).SetStackable(true);

    private static Texture WATER_TEX = ResourceLoader.Load("res://Images/itemWater.png") as Texture;
    public static Item water = new ItemDrink(6, "Water", WATER_TEX, 0.4f);
    

    private static Texture BACTERIA_FOSSIL_TEX = ResourceLoader.Load("res://Images/itemBacteriaFossil.png") as Texture;
    public static Item oxygenBacteriaFossil = 

        new ItemBlock((byte)ItemID.OXYGEN_BACTERIA_FOSSIL, "OxygenBacteriaFossil", BACTERIA_FOSSIL_TEX, Game.GetBlockId<OxygenBacteriaFossilBlock>()).SetStackable(true);
    public static Item nitrogenBacteriaFossil = 
        new ItemBlock((byte)ItemID.NITROGEN_BACTERIA_FOSSIL, "NitrogenBacteriaFossil", BACTERIA_FOSSIL_TEX, Game.GetBlockId<NitrogenBacteriaFossilBlock>()).SetStackable(true);
    public static Item carbonDioxideBacteriaFossil = 
        new ItemBlock((byte)ItemID.CARBON_DIOXIDE_BACTERIA_FOSSIL, "CarbonDioxideBacteriaFossil", BACTERIA_FOSSIL_TEX, Game.GetBlockId<CarbonDioxideBacteriaFossilBlock>()).SetStackable(true);

    private static Texture OXYGEN_BACTERIA_VIAL_TEX = ResourceLoader.Load("res://Images/itemOxygenBacteriaVial.png") as Texture;
    public static Item oxygenBacteriaVial = 
        new ItemBacteriaVial((byte)ItemID.OXYGEN_BACTERIA_VIAL, "OxygenBacteriaVial", OXYGEN_BACTERIA_VIAL_TEX);

    private static Texture NITROGEN_BACTERIA_VIAL_TEX = ResourceLoader.Load("res://Images/itemNitrogenBacteriaVial.png") as Texture;
    public static Item nitrogenBacteriaVial = 
        new ItemBacteriaVial((byte)ItemID.NITROGEN_BACTERIA_VIAL, "NitrogenBacteriaVial", NITROGEN_BACTERIA_VIAL_TEX);
    
    private static Texture CARBON_DIOXIDE_BACTERIA_VIAL_TEX = ResourceLoader.Load("res://Images/itemCarbonDioxideBacteriaVial.png") as Texture;
    public static Item carbonDioxideBacteriaVial = 
        new ItemBacteriaVial((byte)ItemID.CARBON_DIOXIDE_BACTERIA_VIAL, "CarbonDioxideBacteriaVial", CARBON_DIOXIDE_BACTERIA_VIAL_TEX);

    public static void RegisterBlockItem(byte id, Item item)
    {
        blockItems[id] = item;
    }

    public static Item GetItemFromBlock(byte b)
    {
        return blockItems[b];
    }
}