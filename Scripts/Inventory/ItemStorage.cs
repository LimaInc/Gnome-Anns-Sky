using System;
using Godot;

public class ItemStorage
{
    private static Texture BLOCK_TEX = ResourceLoader.Load("res://Images/itemBlock.png") as Texture;
    public static Item block = new Item(1, "Block", BLOCK_TEX, Item.Type.BLOCK).SetStackable(true);

    private static Texture FOSSIL_TEX = ResourceLoader.Load("res://Images/itemFossil.png") as Texture;
    public static Item fossil = new Item(2, "Fossil", FOSSIL_TEX, Item.Type.FOSSIL);

    private static Texture CHOCOLATE_TEX = ResourceLoader.Load("res://Images/itemChocolate.png") as Texture;
    public static Item chocoloate = new Item(3, "Chocolate", CHOCOLATE_TEX, Item.Type.CONSUMABLE);

    private static Texture CAKE_TEX = ResourceLoader.Load("res://Images/itemCake.png") as Texture;
    public static Item cake = new Item(4, "Cake", CAKE_TEX, Item.Type.CONSUMABLE);
}