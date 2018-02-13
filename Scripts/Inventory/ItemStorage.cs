using System;
using Godot;

public class ItemStorage
{
    private static Texture BLOCK_TEX = ResourceLoader.Load("res://Images/itemBlock.png") as Texture;
    public static Item redRock = new ItemBlock(1, "Block", BLOCK_TEX, Game.GetBlockId<RedRock>()).SetStackable(true);

    private static Texture FOSSIL_TEX = ResourceLoader.Load("res://Images/itemFossil.png") as Texture;
    public static Item fossil = new Item(2, "Fossil", FOSSIL_TEX, Item.Type.FOSSIL);

    private static Texture CHOCOLATE_TEX = ResourceLoader.Load("res://Images/itemChocolate.png") as Texture;
    public static Item chocoloate = new ItemFood(3, "Chocolate", CHOCOLATE_TEX, 0.2f);

    private static Texture CAKE_TEX = ResourceLoader.Load("res://Images/itemCake.png") as Texture;
    public static Item cake = new ItemFood(4, "Cake", CAKE_TEX, 0.5f);

    public static Item GetItemFromBlock(byte b)
    {
        switch (b)
        {
            case 2: return redRock;
            default: return null;
        }
    }
}