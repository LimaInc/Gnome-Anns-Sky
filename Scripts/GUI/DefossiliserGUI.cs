using System;
using System.Collections.Generic;
using Godot;

// TODO: remove most of this class, this is very not DRY as most of it is already in InventoryGUI
public class DefossiliserGUI : GUI
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
    private IDictionary<Item.Type, Inventory> subInventories;

    private GUIBox box;

    private GUIFloatingSlot floatingSlot;
    private GUIInventorySlotArray inSlotArray;
    private GUIVerticalBar progressBar;
    private GUIInventorySlotArray outSlotArray;
    private IDictionary<Item.Type, GUILabeledSlotArray> subArrays = new Dictionary<Item.Type, GUILabeledSlotArray>
    {
        [Item.Type.BLOCK] = null,
        [Item.Type.CONSUMABLE] = null,
        [Item.Type.FOSSIL] = null
    };

    private static readonly IDictionary<Item.Type, String> subInventoryNames = new Dictionary<Item.Type, String>
    {
        [Item.Type.BLOCK] = "Blocks",
        [Item.Type.CONSUMABLE] = "Consumables",
        [Item.Type.FOSSIL] = "Fossils"
    };
    private static readonly IDictionary<Item.Type, int> subInvIndices = new Dictionary<Item.Type, int>
    {
        [Item.Type.CONSUMABLE] = 0,
        [Item.Type.FOSSIL] = 1,
        [Item.Type.BLOCK] = 2
    };
    private static readonly IDictionary<Item.Type, Label> subInvLabels = new Dictionary<Item.Type, Label>
    {
        [Item.Type.BLOCK] = null,
        [Item.Type.CONSUMABLE] = null,
        [Item.Type.FOSSIL] = null
    };

    public DefossiliserGUI(Defossiliser defossiliser, Player p, 
        IDictionary<Item.Type,Inventory> inventories, Node vSource) : base(vSource)
    {
        this.defossiliser = defossiliser;
        this.subInventories = inventories;
        this.player = p;

        Initialize();
    }

    private void Initialize()
    {
        this.Hide();

        Vector2 empty = new Vector2();
        floatingSlot = new GUIFloatingSlot();
        inSlotArray = new GUIInventorySlotArray(floatingSlot, Item.Type.ANY, IN_SLOT_COUNT, empty);
        progressBar = new GUIVerticalBar(empty, PROGRESS_BAR_HEIGHT, new Color(0, 0.6f, 0));
        progressBar.Rotate(Mathf.PI);
        outSlotArray = new GUIInventorySlotArray(floatingSlot, Item.Type.ANY, OUT_SLOT_COUNT, empty);
        foreach (Item.Type type in new List<Item.Type>(subArrays.Keys))
        {
            subArrays[type] = new GUILabeledSlotArray(floatingSlot, type, subInventoryNames[type], INVENTORY_SLOT_COUNT, 
                empty, empty);
        }
        box = new GUIBox(this.GetViewportDimensions() / 2, BOX_SIZE);
        this.AddChild(box);
        foreach (GUILabeledSlotArray slotArr in subArrays.Values)
        {
            this.box.AddChild(slotArr);
        }
        this.box.AddChild(inSlotArray);
        this.box.AddChild(progressBar);
        this.box.AddChild(outSlotArray);
        this.AddChild(floatingSlot);
    }

    private void UpdateSlots()
    {
        UpdateDefossiliserSlotState();
        foreach (KeyValuePair<Item.Type, GUILabeledSlotArray> kvPair in subArrays)
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
        foreach (KeyValuePair<Item.Type, GUILabeledSlotArray> kvPair in subArrays)
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
        box.Position = this.GetViewportDimensions() / 2;

        Vector2 perSlotSize = SLOT_SPACING + GUIInventorySlot.SIZE;
        Vector2 subArraySize = INVENTORY_SLOT_COUNT * perSlotSize;
        float defossiliserArrayWidth = Mathf.Max(IN_SLOT_COUNT.x, OUT_SLOT_COUNT.x) * perSlotSize.x;
        float sectionSpacing = (BOX_SIZE.x - subArrays.Count * subArraySize.x - SLOT_OFFSET.x * 2 -
            defossiliserArrayWidth) / subArrays.Count;

        Vector2 offset = SLOT_OFFSET - BOX_SIZE / 2;
        Vector2 delta = new Vector2(subArraySize.x + sectionSpacing, 0);

        foreach (KeyValuePair<Item.Type, GUILabeledSlotArray> kvPair in subArrays)
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
        this.Show();
        Input.SetMouseMode(Input.MouseMode.Visible);
        UpdateSlots();
        defossiliser.Callback = UpdateDefossiliserSlotState;
    }

    public override void HandleClose()
    {
        this.Hide();
        SaveSlotState();
        defossiliser.Callback = null;
        ItemStack stack = floatingSlot.GetCurItemStack();
        if (stack != null)
        {
            this.subInventories[stack.GetItem().GetType()].TryAddItemStack(stack);
            floatingSlot.ClearItemStack();
        }
    }

    public override void _Ready()
    {
        base._Ready();
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

        if (e is InputEventMouseMotion iemb)
        {
            SaveDefossiliserSlotState();
        }
    }
    
    public override void _Process(float delta)
    {
        progressBar.SetPercentage(defossiliser.DefossilisingProgress);
    }
}