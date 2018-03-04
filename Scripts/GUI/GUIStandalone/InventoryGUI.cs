using System;
using System.Collections.Generic;
using Godot;

public class InventoryGUI : GUI
{
    public const int BOX_Z = 0;
    public const int ARRAY_SLOT_Z = 1;
    public const int HAND_SLOT_Z = 1;
    public const int FLOATING_SLOT_Z = 5;

    private static readonly IntVector2 SLOT_COUNT = new IntVector2(4,10);

    private static readonly Vector2 SLOT_SPACING = new Vector2(2, 2);
    private static readonly Vector2 SLOT_OFFSET = new Vector2(20.0f, 30.0f);
    private static readonly Vector2 BOX_SIZE = new Vector2(550.0f, 400.0f);
    private static readonly Vector2 HAND_SLOT_OFFSET = new Vector2(-16.0f, 170.0f);
    private static readonly Vector2 LABEL_SHIFT = new Vector2(0, -16);

    private IDictionary<Item.ItemType, Inventory> subInventories = new Dictionary<Item.ItemType, Inventory>
    {
        [Item.ItemType.BLOCK] = null,
        [Item.ItemType.CONSUMABLE] = null,
        [Item.ItemType.PROCESSED] = null
    };

    // This follows the mouse to allow the player to move items around
    private GUIInventorySlot floatingSlot;

    private IDictionary<Item.ItemType, GUILabeledSlotArray> subInvSlots = new Dictionary<Item.ItemType, GUILabeledSlotArray>
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

    private GUIInventorySlot handSlot;

    private GUIBox box;

    private Player player;

    public InventoryGUI(Player player_, IDictionary<Item.ItemType,Inventory> inventories, Node vSource) : base(vSource)
    {
        player = player_;
        subInventories = inventories;

        Initialize();
    }

    public void UpdateHandSlot()
    {
    }

    public override void HandleResize()
    {
        box.Position = GetViewportDimensions() / 2;
        
        Vector2 sectionSize = SLOT_COUNT * (SLOT_SPACING + GUIInventorySlot.SIZE);
        float sectionSpacing = (BOX_SIZE.x - subInvSlots.Count * sectionSize.x - 2 * SLOT_OFFSET.x) / (subInvSlots.Count - 1);

        Vector2 offset = SLOT_OFFSET - BOX_SIZE / 2;
        Vector2 delta = new Vector2(sectionSize.x + sectionSpacing, 0.0f);

        foreach (KeyValuePair<Item.ItemType, GUILabeledSlotArray> kvPair in subInvSlots)
        {
            kvPair.Value.SetPosition(offset + delta * subInvIndices[kvPair.Key]);
            kvPair.Value.SetSize(SLOT_SPACING, LABEL_SHIFT);
        }
    }

    private void Initialize()
    {
        Hide();

        floatingSlot = new GUIFloatingSlot
        {
            ZAsRelative = true,
            ZIndex = FLOATING_SLOT_Z
        };

        // this is rather ad-hoc and in general unsafe without thinking about how the slots are synchronized with the inventory
        // TODO: short-term think of a better solution here
        // TODO: long-term think of a better slot sync scheme
        bool offerToMainInventory(ItemStack iStack)
        {
            SaveSlotState();
            bool result = subInventories[iStack.Item.IType].TryAddItemStack(iStack);
            UpdateSlots();
            return result;
        }
        handSlot = new GUIInventorySlot(floatingSlot, Item.ItemType.ANY, -2, HAND_SLOT_OFFSET, offerToMainInventory)
        {
            ZAsRelative = true,
            ZIndex = HAND_SLOT_Z
        };

        foreach (Item.ItemType type in new List<Item.ItemType>(subInvSlots.Keys))
        {
            Vector2 empty = new Vector2();
            
            subInvSlots[type] = new GUILabeledSlotArray(floatingSlot, type, subInventoryNames[type], SLOT_COUNT,
                empty, empty, iStack => handSlot.OfferItemStack(iStack) == null)
            {
                ZAsRelative = true,
                ZIndex = ARRAY_SLOT_Z
            };
        }

        box = new GUIBox(GetViewportDimensions() / 2, BOX_SIZE)
        {
            ZAsRelative = true,
            ZIndex = BOX_Z
        };

        AddChild(box);
        foreach (GUILabeledSlotArray slotArr in subInvSlots.Values)
        {
            box.AddChild(slotArr);
        }
        box.AddChild(handSlot);
        AddChild(floatingSlot);
    }

    private void UpdateSlots()
    {
        foreach (KeyValuePair<Item.ItemType, GUILabeledSlotArray> kvPair in subInvSlots)
        {
            kvPair.Value.OverrideFromInventory(subInventories[kvPair.Key]);
        }
        handSlot.AssignItemStack(player.ItemInHand);
    }

    private void SaveSlotState()
    {
        foreach (KeyValuePair<Item.ItemType, GUILabeledSlotArray> kvPair in subInvSlots)
        {
            kvPair.Value.SaveToInventory(subInventories[kvPair.Key]);
        }
        player.ItemInHand = handSlot.GetCurItemStack();
    }

    public override void HandleOpen(Node parent)
    {
        HandleResize();
        UpdateSlots();
        Show();
        Input.SetMouseMode(Input.MouseMode.Visible);
    }

    public override void HandleClose()
    {
        Hide();
        SaveSlotState();
        ItemStack stack = floatingSlot.GetCurItemStack();
        if (stack != null)
        {
            this.subInventories[stack.Item.IType].TryAddItemStack(stack);
            floatingSlot.ClearItemStack();
        }
    }

    public override void _Input(InputEvent e)
    {
        if (e is InputEventMouseMotion iemm)
        {
            floatingSlot.SetPosition(iemm.GetPosition());
        }
    }
}