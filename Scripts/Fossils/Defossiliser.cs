using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using static ItemID;

public class Defossiliser : Node
{
    public const int IN_INVENTORY_SIZE = 9;
    public const int OUT_INVENTORY_SIZE = 9;
    public readonly IList<DefossiliserAction> DEFAULT_PROCESSES = new List<DefossiliserAction>
    {
        new DefossiliserAction(ICE, WATER, processingTime: 5, outItemCount: 2),
        new DefossiliserAction(OXYGEN_BACTERIA_FOSSIL, OXYGEN_BACTERIA_VIAL, processingTime: 12),
        new DefossiliserAction(NITROGEN_BACTERIA_FOSSIL, NITROGEN_BACTERIA_VIAL, processingTime: 10),
        new DefossiliserAction(CARBON_DIOXIDE_BACTERIA_FOSSIL, CARBON_DIOXIDE_BACTERIA_VIAL, processingTime: 7),
        new DefossiliserAction(GRASS_FOSSIL, GRASS, processingTime: 15),
        new DefossiliserAction(TREE_FOSSIL, TREE, processingTime: 30, inItemCount: 3),
        new DefossiliserAction(FROG_FOSSIL, FROG_EGG, processingTime: 40, inItemCount: 2),
        new DefossiliserAction(REGULAR_ANIMAL_FOSSIL, REGULAR_EGG, processingTime: 60, inItemCount: 3),
        new DefossiliserAction(BIG_ANIMAL_FOSSIL, BIG_EGG, processingTime: 120, inItemCount: 5),
        new DefossiliserAction(WHEAT, CAKE, processingTime: 120, inItemCount: 5)
    };
    
    public Inventory OutInventory { get; private set; }
    public Inventory InInventory { get; private set; }

    public float DefossilisingProgress { get; private set; }
    public DefossiliserAction ActionInProgress { get; private set; }
    
    public Action Callback { get; set; }

    private readonly IList<DefossiliserAction> possibleProcesses;

    public Defossiliser(IList<DefossiliserAction> initialProcesses = null)
    {
        possibleProcesses = new List<DefossiliserAction>();
        if (initialProcesses != null)
        {
            AddProcesses(initialProcesses);
        }
        DefossilisingProgress = 0;

        InInventory = new Inventory(Item.ItemType.ANY, IN_INVENTORY_SIZE);
        OutInventory = new Inventory(Item.ItemType.ANY, OUT_INVENTORY_SIZE);
    }

    public void AddProcesses(IList<DefossiliserAction> newProcesses)
    {
        foreach(DefossiliserAction da in newProcesses)
        {
            if (!possibleProcesses.Contains(da))
            {
                possibleProcesses.Add(da);
            }
        }
    }

    // VERY hacky
    // TODO: fix after rewriting block system
    public override void _Ready()
    {
        if (possibleProcesses == null)
        {
            AddProcesses(DEFAULT_PROCESSES);
        }
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
                possibleProcesses.Where(p => p.CanBeDoneWith(InInventory) && OutInventory.CanAdd(p.outItemID, p.outItemCount));
            if (doableActionsRightNow.Any())
            {
                ActionInProgress = doableActionsRightNow.OrderBy(action => action.ProcessingTime).First();
            }
        }
        else
        {
            if (ActionInProgress.CanBeDoneWith(InInventory) && 
                OutInventory.CanAdd(ActionInProgress.outItemID, ActionInProgress.outItemCount))
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
