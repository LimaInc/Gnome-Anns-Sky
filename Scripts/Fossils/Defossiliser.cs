using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;

public class Defossiliser
{
    public const int IN_INVENTORY_SIZE = 6;
    public const int OUT_INVENTORY_SIZE = 6;

    public Inventory OutInventory { get; private set; }
    public Inventory InInventory { get; private set; }

    private static readonly IDictionary<Item, DefossiliserAction> transforms = new Dictionary<Item, DefossiliserAction>
    {
        [ItemStorage.nitrogenBacteriaFossil] = 
            new DefossiliserAction(ItemStorage.nitrogenBacteriaFossil, ItemStorage.nitrogenBacteriaVial),
        [ItemStorage.oxygenBacteriaFossil] =
            new DefossiliserAction(ItemStorage.oxygenBacteriaFossil, ItemStorage.oxygenBacteriaVial),
        [ItemStorage.carbonDioxideBacteriaFossil] =
            new DefossiliserAction(ItemStorage.carbonDioxideBacteriaFossil, ItemStorage.carbonDioxideBacteriaVial),
    };

    public Defossiliser()
    {
        InInventory = new Inventory(Item.Type.ANY, IN_INVENTORY_SIZE);
        InInventory = new Inventory(Item.Type.ANY, OUT_INVENTORY_SIZE);
    }

    public void HandleInput(InputEvent e, Player p)
    {
        if (e is InputEventMouseButton iemb && InputUtil.IsRighPress(iemb))
        {
            // breaks encapsulation of Inventories in player, TODO: fix
            p.OpenGUI(new DefossiliserGUI(this, p, p.Inventories, p));
        }
    }
}
