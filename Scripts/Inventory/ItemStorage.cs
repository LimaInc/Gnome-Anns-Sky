using System;
using Godot;

public class ItemStorage
{
    enum ItemID : byte {
        BLOCK, CHOCOLATE,
        OXYGEN_BACTERIA_FOSSIL, NITROGEN_BACTERIA_FOSSIL, CARBON_DIOXIDE_BACTERIA_FOSSIL,
        OXYGEN_BACTERIA_VIAL, NITROGEN_BACTERIA_VIAL, CARBON_DIOXIDE_BACTERIA_VIAL
    }
    private static Texture BLOCK_TEX = ResourceLoader.Load("res://Images/itemBlock.png") as Texture;
    public static Item block = new Item((byte) ItemID.BLOCK, "Block", BLOCK_TEX, Item.Type.BLOCK).SetStackable(true);

    private static Texture CHOCOLATE_TEX = ResourceLoader.Load("res://Images/itemChocolate.png") as Texture;
    public static Item chocoloate = new Item((byte)ItemID.CHOCOLATE, "Chocolate", CHOCOLATE_TEX, Item.Type.CONSUMABLE);

    private static Texture BACTERIA_FOSSIL_TEX = ResourceLoader.Load("res://Images/itemBacteriaFossil.png") as Texture;
    public static Item oxygenBacteriaFossil = 
        new Item((byte)ItemID.OXYGEN_BACTERIA_FOSSIL, "OxygenBacteriaFossil", BACTERIA_FOSSIL_TEX, Item.Type.BLOCK).SetStackable(true);
    public static Item nitrogenBacteriaFossil = 
        new Item((byte)ItemID.NITROGEN_BACTERIA_FOSSIL, "NitrogenBacteriaFossil", BACTERIA_FOSSIL_TEX, Item.Type.BLOCK).SetStackable(true);
    public static Item carbonDioxideBacteriaFossil = 
        new Item((byte)ItemID.CARBON_DIOXIDE_BACTERIA_FOSSIL, "CarbonDioxideBacteriaFossil", BACTERIA_FOSSIL_TEX, Item.Type.BLOCK).SetStackable(true);

    private static Texture OXYGEN_BACTERIA_VIAL_TEX = ResourceLoader.Load("res://Images/itemOxygenBacteriaVial.png") as Texture;
    public static Item oxygenBacteriaVial = 
        new Item((byte)ItemID.OXYGEN_BACTERIA_VIAL, "OxygenBacteriaVial", OXYGEN_BACTERIA_VIAL_TEX, Item.Type.FOSSIL);

    private static Texture NITROGEN_BACTERIA_VIAL_TEX = ResourceLoader.Load("res://Images/itemNitrogenBacteriaVial.png") as Texture;
    public static Item nitrogenBacteriaVial = 
        new Item((byte)ItemID.NITROGEN_BACTERIA_VIAL, "NitrogenBacteriaVial", NITROGEN_BACTERIA_VIAL_TEX, Item.Type.FOSSIL);
    
    private static Texture CARBON_DIOXIDE_BACTERIA_VIAL_TEX = ResourceLoader.Load("res://Images/itemCarbonDioxideBacteriaVial.png") as Texture;
    public static Item carbonDioxideBacteriaVial = 
        new Item((byte)ItemID.CARBON_DIOXIDE_BACTERIA_VIAL, "CarbonDioxideBacteriaVial", CARBON_DIOXIDE_BACTERIA_VIAL_TEX, Item.Type.FOSSIL);
}