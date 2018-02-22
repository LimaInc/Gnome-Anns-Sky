using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Defossiliser
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

    private DefossiliserGUI defossiliserGUI;

    public Defossiliser()
    {
        defossiliserGUI = null;
    }
}
