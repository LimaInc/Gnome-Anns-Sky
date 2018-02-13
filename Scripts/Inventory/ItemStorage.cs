using System;
using Godot;
using System.Collections.Generic;

public class ItemStorage
{
    private static Dictionary<byte, Item> blockItems = new Dictionary<byte, Item>();

    private static Texture RED_ROCK_TEX = ResourceLoader.Load("res://Images/itemRedRock.png") as Texture;
    public static Item redRock = new ItemBlock(1, "Red Rock", RED_ROCK_TEX, Game.GetBlockId<RedRock>()).SetStackable(true);

    private static Texture FOSSIL_TEX = ResourceLoader.Load("res://Images/itemFossil.png") as Texture;
    public static Item fossil = new Item(2, "Fossil", FOSSIL_TEX, Item.Type.FOSSIL);

    private static Texture CHOCOLATE_TEX = ResourceLoader.Load("res://Images/itemChocolate.png") as Texture;
    public static Item chocoloate = new ItemFood(3, "Chocolate", CHOCOLATE_TEX, 0.2f);

    private static Texture CAKE_TEX = ResourceLoader.Load("res://Images/itemCake.png") as Texture;
    public static Item cake = new ItemFood(4, "Cake", CAKE_TEX, 0.5f);

    private static Texture STONE_BLOCK_TEX = ResourceLoader.Load("res://Images/itemStone.png") as Texture;
    public static Item stone = new ItemBlock(5, "Stone", STONE_BLOCK_TEX, Game.GetBlockId<Stone>()).SetStackable(true);

    public static void RegisterBlockItem(byte id, Item item)
    {
        blockItems[id] = item;
    }

    public static Item GetItemFromBlock(byte b)
    {
        return blockItems[b];
    }
}