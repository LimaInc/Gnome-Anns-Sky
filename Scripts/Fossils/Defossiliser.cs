using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;

public class Defossiliser
{
    private static readonly IDictionary<Item, DefossiliserAction> transforms = new Dictionary<Item, DefossiliserAction>
    {
        [ItemStorage.nitrogenBacteriaFossil] = 
            new DefossiliserAction(ItemStorage.nitrogenBacteriaFossil, ItemStorage.nitrogenBacteriaVial),
        [ItemStorage.oxygenBacteriaFossil] =
            new DefossiliserAction(ItemStorage.oxygenBacteriaFossil, ItemStorage.oxygenBacteriaVial),
        [ItemStorage.carbonDioxideBacteriaFossil] =
            new DefossiliserAction(ItemStorage.carbonDioxideBacteriaFossil, ItemStorage.carbonDioxideBacteriaVial),
    };

    public void HandleInput(InputEvent e, Player p)
    {
        if (e is InputEventMouseButton iemb && InputUtil.IsRighPress(iemb))
        {
            p.OpenGUI(new DefossiliserGUI(this, p.InventoryGUI, p));
        }
    }
}
