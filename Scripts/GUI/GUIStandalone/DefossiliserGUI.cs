using System;
using System.Collections.Generic;
using Godot;

// TODO: remove most of this class, this is very not DRY as most of it is already in InventoryGUI
public class DefossiliserGUI : VisibleMouseGUI
{
    public static readonly IntVector2 IN_SLOT_COUNT = new IntVector2(3, 3);
    public static readonly IntVector2 OUT_SLOT_COUNT = new IntVector2(3, 3);
    private static readonly IntVector2 INVENTORY_SLOT_COUNT = new IntVector2(4, 10);

    private static readonly Vector2 BOX_SIZE = new Vector2(600, 400);
    private static readonly Vector2 SLOT_SPACING = new Vector2(2, 2);
    private static readonly Vector2 SLOT_OFFSET = new Vector2(20, 30);
    private static readonly Vector2 LABEL_SHIFT = new Vector2(0, -16);
    private const float PROGRESS_BAR_HEIGHT = 100;

    private Defossiliser defossiliser;
    private Player player;
    private IDictionary<Item.ItemType, Inventory> subInventories;

    private GUIBox box;

    private GUIFloatingSlot floatingSlot;
    private GUIInventorySlotArray inSlotArray;
    private GUIVerticalBar progressBar;
    private GUIInventorySlotArray outSlotArray;
    private IDictionary<Item.ItemType, GUILabeledSlotArray> subArrays = new Dictionary<Item.ItemType, GUILabeledSlotArray>
    {
        [Item.ItemType.BLOCK] = null,
        [Item.ItemType.CONSUMABLE] = null,
        [Item.ItemType.PROCESSED] = null
    };

    private static readonly IDictionary<Item.ItemType, String> subInventoryNames = new Dictionary<Item.ItemType, String>
    {
        [Item.ItemType.BLOCK] = "Blocks",
        [Item.ItemType.CONSUMABLE] = "Consumables",
        [Item.ItemType.PROCESSED] = "Processed"
    };
    private static readonly IDictionary<Item.ItemType, int> subInvIndices = new Dictionary<Item.ItemType, int>
    {
        [Item.ItemType.CONSUMABLE] = 0,
        [Item.ItemType.PROCESSED] = 1,
        [Item.ItemType.BLOCK] = 2
    };
    private static readonly IDictionary<Item.ItemType, Label> subInvLabels = new Dictionary<Item.ItemType, Label>
    {
        [Item.ItemType.BLOCK] = null,
        [Item.ItemType.CONSUMABLE] = null,
        [Item.ItemType.PROCESSED] = null
    };

    public DefossiliserGUI(Defossiliser defossiliserMachine, Player p, 
        IDictionary<Item.ItemType,Inventory> inventories, Node vSource) : base(vSource)
    {
        defossiliser = defossiliserMachine;
        subInventories = inventories;
        player = p;

        Initialize();
    }

    // TOOD: inventory / GUI synchronization is a mess
    // rewrite it (shouldn't change too much,
    //              mainly SaveSlots() and UpdateSlots() methods and immediate neighbours)
    // using delegates / lambdas / anonymous functions / whatever your favorite term is
    private void Initialize()
    {
        Hide();

        Vector2 empty = new Vector2();

        bool offerToMainInventory(ItemStack iStack)
        {
            SaveSlotState();
            bool result = subInventories[iStack.Item.IType].TryAddItemStack(iStack);
            UpdateSlots();
            return result;
        }

        floatingSlot = new GUIFloatingSlot();
        inSlotArray = new GUIInventorySlotArray(floatingSlot, Item.ItemType.ANY, IN_SLOT_COUNT, empty,
            offerToMainInventory,
            () => inSlotArray.SaveToInventory(defossiliser.InInventory));
        progressBar = new GUIVerticalBar(empty, PROGRESS_BAR_HEIGHT, new Color(0, 0.6f, 0));
        progressBar.Rotate(Mathf.PI);
        outSlotArray = new GUIInventorySlotArray(floatingSlot, Item.ItemType.ANY, OUT_SLOT_COUNT, empty, 
            offerToMainInventory, 
            () => outSlotArray.SaveToInventory(defossiliser.OutInventory));

        bool offerToInputInventory(ItemStack iStack)
        {
            SaveSlotState();
            bool result = defossiliser.InInventory.TryAddItemStack(iStack);
            UpdateSlots();
            return result;
        }

        foreach (Item.ItemType type in new List<Item.ItemType>(subArrays.Keys))
        {
            subArrays[type] = new GUILabeledSlotArray(floatingSlot, type, subInventoryNames[type], INVENTORY_SLOT_COUNT, 
                empty, empty, offerToInputInventory);
        }
        box = new GUIBox(this.GetViewportDimensions() / 2, BOX_SIZE);
        AddChild(box);
        foreach (GUILabeledSlotArray slotArr in subArrays.Values)
        {
            box.AddChild(slotArr);
        }
        box.AddChild(inSlotArray);
        box.AddChild(progressBar);
        box.AddChild(outSlotArray);
        AddChild(floatingSlot);
    }

    private void UpdateSlots()
    {
        UpdateDefossiliserSlotState();
        foreach (KeyValuePair<Item.ItemType, GUILabeledSlotArray> kvPair in subArrays)
        {
            kvPair.Value.OverrideFromInventory(subInventories[kvPair.Key]);
        }
    }

    private void UpdateDefossiliserSlotState()
    {
        inSlotArray.OverrideFromInventory(defossiliser.InInventory);
        outSlotArray.OverrideFromInventory(defossiliser.OutInventory);
    }

    private void SaveSlotState()
    {
        SaveDefossiliserSlotState();
        foreach (KeyValuePair<Item.ItemType, GUILabeledSlotArray> kvPair in subArrays)
        {
            kvPair.Value.SaveToInventory(subInventories[kvPair.Key]);
        }
    }

    private void SaveDefossiliserSlotState()
    {
        inSlotArray.SaveToInventory(defossiliser.InInventory);
        outSlotArray.SaveToInventory(defossiliser.OutInventory);
    }

    public override void HandleResize()
    {
        box.Position = GetViewportDimensions() / 2;

        Vector2 perSlotSize = SLOT_SPACING + GUIInventorySlot.SIZE;
        Vector2 subArraySize = INVENTORY_SLOT_COUNT * perSlotSize;
        float defossiliserArrayWidth = Mathf.Max(IN_SLOT_COUNT.x, OUT_SLOT_COUNT.x) * perSlotSize.x;
        float sectionSpacing = (BOX_SIZE.x - subArrays.Count * subArraySize.x - SLOT_OFFSET.x * 2 -
            defossiliserArrayWidth) / subArrays.Count;

        Vector2 offset = SLOT_OFFSET - BOX_SIZE / 2;
        Vector2 delta = new Vector2(subArraySize.x + sectionSpacing, 0);

        foreach (KeyValuePair<Item.ItemType, GUILabeledSlotArray> kvPair in subArrays)
        {
            kvPair.Value.SetPosition(offset + delta * subInvIndices[kvPair.Key]);
            kvPair.Value.SetSize(SLOT_SPACING, LABEL_SHIFT);
        }

        inSlotArray.Position = offset + delta * subArrays.Count;
        inSlotArray.SetSize(SLOT_SPACING);

        progressBar.Position = progressBar.Size / 2 + 
            new Vector2((offset + delta * subArrays.Count).x + defossiliserArrayWidth / 2, 0);

        outSlotArray.Position = new Vector2((offset + delta * subArrays.Count).x, 
            (BOX_SIZE/2 - OUT_SLOT_COUNT * perSlotSize - SLOT_OFFSET).y);
        outSlotArray.SetSize(SLOT_SPACING);
    }

    public override void HandleOpen(Node parent)
    {
        base.HandleOpen(parent);
        HandleResize();
        Show();
        UpdateSlots();
        defossiliser.Callback = UpdateDefossiliserSlotState;
    }

    public override void HandleClose()
    {
        base.HandleClose();
        Hide();
        SaveSlotState();
        defossiliser.Callback = null;
        ItemStack stack = floatingSlot.GetCurItemStack();
        if (stack != null)
        {
            subInventories[stack.Item.IType].TryAddItemStack(stack);
            floatingSlot.ClearItemStack();
        }
    }

    public override void _Ready()
    {
        Game g = GetNode(Game.GAME_PATH) as Game;
        if (!g.IsAParentOf(defossiliser))
        {
            g.AddChild(defossiliser);
        }
    }

    public override void _Input(InputEvent e)
    {
        if (e is InputEventMouseMotion iemm)
        {
            floatingSlot.SetPosition(iemm.GetPosition());
        }

        if (e is InputEventMouseButton iemb)
        {
            SaveDefossiliserSlotState();
        }
    }
    
    public override void _Process(float delta)
    {
        progressBar.Percentage = defossiliser.DefossilisingProgress;
    }
}