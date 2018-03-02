using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;

public class Defossiliser : Node
{
    public const int IN_INVENTORY_SIZE = 9;
    public const int OUT_INVENTORY_SIZE = 9;

    public readonly IList<DefossiliserAction> possibleProcesses;
    public static readonly IList<DefossiliserAction> DEFAULT_PROCESSES = new List<DefossiliserAction>
    {
        new DefossiliserAction(ItemStorage.oxygenBacteriaFossil, ItemStorage.oxygenBacteriaVial, processingTime: 12),
        new DefossiliserAction(ItemStorage.nitrogenBacteriaFossil, ItemStorage.nitrogenBacteriaVial, processingTime: 10),
        new DefossiliserAction(ItemStorage.carbonDioxideBacteriaFossil, ItemStorage.carbonDioxideBacteriaVial, processingTime: 7),
        new DefossiliserAction(ItemStorage.grassFossil, ItemStorage.grass, processingTime: 15),
        new DefossiliserAction(ItemStorage.treeFossil, ItemStorage.tree, processingTime: 30),
        new DefossiliserAction(ItemStorage.ice, ItemStorage.water, processingTime: 5, outItemCount: 2),
    };

    public Inventory OutInventory { get; private set; }
    public Inventory InInventory { get; private set; }

    public float DefossilisingProgress { get; private set; }
    public DefossiliserAction ActionInProgress { get; private set; }

    public delegate void InventoryChangeHandler();

    public InventoryChangeHandler Callback { get; set; }

    private static readonly IDictionary<Item, DefossiliserAction> transforms = new Dictionary<Item, DefossiliserAction>
    {
        [ItemStorage.nitrogenBacteriaFossil] = 
            new DefossiliserAction(ItemStorage.nitrogenBacteriaFossil, ItemStorage.nitrogenBacteriaVial),
        [ItemStorage.oxygenBacteriaFossil] =
            new DefossiliserAction(ItemStorage.oxygenBacteriaFossil, ItemStorage.oxygenBacteriaVial),
        [ItemStorage.carbonDioxideBacteriaFossil] =
            new DefossiliserAction(ItemStorage.carbonDioxideBacteriaFossil, ItemStorage.carbonDioxideBacteriaVial),
    };

    public Defossiliser(IList<DefossiliserAction> possibleProcesses = null)
    {
        this.possibleProcesses = possibleProcesses ?? DEFAULT_PROCESSES;
        DefossilisingProgress = 0;

        InInventory = new Inventory(Item.ItemType.ANY, IN_INVENTORY_SIZE);
        OutInventory = new Inventory(Item.ItemType.ANY, OUT_INVENTORY_SIZE);
    }

    public void HandleInput(InputEvent e, Player p)
    {
        if (e is InputEventMouseButton iemb && InputUtil.IsRighPress(iemb))
        {
            // breaks encapsulation of Inventories in player, TODO: fix
            p.OpenGUI(new DefossiliserGUI(this, p, p.Inventories, p));
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        delta *= Game.SPEED;
        if (ActionInProgress == null)
        {
            IEnumerable<DefossiliserAction> doableActionsRightNow = 
                possibleProcesses.Where(p => p.CanBeDoneWith(InInventory) && OutInventory.CanAdd(p.outItem, p.outItemCount));
            if (doableActionsRightNow.Any())
            {
                ActionInProgress = doableActionsRightNow.OrderBy(action => action.ProcessingTime).First();
            }
        }
        else
        {
            if (ActionInProgress.CanBeDoneWith(InInventory) && 
                OutInventory.CanAdd(ActionInProgress.outItem, ActionInProgress.outItemCount))
            {
                DefossilisingProgress += delta / ActionInProgress.ProcessingTime;
                if (DefossilisingProgress >= 1)
                {
                    OutInventory.TryAddItemStack(ActionInProgress.Process(InInventory));
                    Callback?.Invoke();
                    ActionInProgress = null;
                    DefossilisingProgress = 0;
                }
            }
            else
            {
                ActionInProgress = null;
                DefossilisingProgress = 0;
            }
        }
            
    }
}
