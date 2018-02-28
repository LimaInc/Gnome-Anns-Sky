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

    private IDictionary<Item.Type, Inventory> subInventories = new Dictionary<Item.Type, Inventory>
    {
        [Item.Type.BLOCK] = null,
        [Item.Type.CONSUMABLE] = null,
        [Item.Type.FOSSIL] = null
    };

    // This follows the mouse to allow the player to move items around
    private GUIInventorySlot floatingSlot;

    private IDictionary<Item.Type, GUILabeledSlotArray> subInvSlots = new Dictionary<Item.Type, GUILabeledSlotArray>
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

    private GUIInventorySlot handSlot;

    private GUIBox box;

    private Player player;

    public InventoryGUI(Player player, IDictionary<Item.Type,Inventory> inventories, Node vSource) : base(vSource)
    {
        this.player = player;
        this.subInventories = inventories;

        this.Initialize();
    }

    public void UpdateHandSlot()
    {
        this.handSlot.AssignItemStack(player.ItemInHand);
    }

    public override void HandleResize()
    {
        box.Position = this.GetViewportDimensions() / 2;
        
        Vector2 sectionSize = SLOT_COUNT * (SLOT_SPACING + GUIInventorySlot.SIZE);
        float sectionSpacing = (BOX_SIZE.x - subInvSlots.Count * sectionSize.x - 2 * SLOT_OFFSET.x) / (subInvSlots.Count - 1);

        Vector2 offset = SLOT_OFFSET - BOX_SIZE / 2;
        Vector2 delta = new Vector2(sectionSize.x + sectionSpacing, 0.0f);

        foreach (KeyValuePair<Item.Type, GUILabeledSlotArray> kvPair in subInvSlots)
        {
            kvPair.Value.SetPosition(offset + delta * subInvIndices[kvPair.Key]);
            kvPair.Value.SetSize(SLOT_SPACING, LABEL_SHIFT);
        }
    }

    private void Initialize()
    {
        this.Hide();

        floatingSlot = new GUIFloatingSlot
        {
            ZAsRelative = true,
            ZIndex = FLOATING_SLOT_Z
        };

        foreach (Item.Type type in new List<Item.Type>(subInvSlots.Keys))
        {
            Vector2 empty = new Vector2();
            subInvSlots[type] = new GUILabeledSlotArray(floatingSlot, type, subInventoryNames[type], SLOT_COUNT,
                empty, empty)
            {
                ZAsRelative = true,
                ZIndex = ARRAY_SLOT_Z
            };
        }

        box = new GUIBox(this.GetViewportDimensions() / 2, BOX_SIZE)
        {
            ZAsRelative = true,
            ZIndex = BOX_Z
        };

        handSlot = new GUIInventorySlot(floatingSlot, Item.Type.ANY, -2, HAND_SLOT_OFFSET)
        {
            ZAsRelative = true,
            ZIndex = HAND_SLOT_Z
        };
        handSlot.AssignItemStack(player.ItemInHand);

        this.AddChild(box);
        foreach (GUILabeledSlotArray slotArr in subInvSlots.Values)
        {
            this.box.AddChild(slotArr);
        }
        this.box.AddChild(handSlot);
        this.AddChild(floatingSlot);
    }

    private void UpdateSlots()
    {
        foreach (KeyValuePair<Item.Type, GUILabeledSlotArray> kvPair in subInvSlots)
        {
            kvPair.Value.OverrideFromInventory(subInventories[kvPair.Key]);
        }
    }

    private void SaveSlotState()
    {
        foreach (KeyValuePair<Item.Type, GUILabeledSlotArray> kvPair in subInvSlots)
        {
            kvPair.Value.SaveToInventory(subInventories[kvPair.Key]);
        }
        player.ItemInHand = handSlot.GetCurItemStack();
    }

    public override void HandleOpen(Node parent)
    {
        UpdateSlots();
        this.Show();
        Input.SetMouseMode(Input.MouseMode.Visible);
    }

    public override void HandleClose()
    {
        this.Hide();
        SaveSlotState();
        ItemStack stack = floatingSlot.GetCurItemStack();
        if (stack != null)
        {
            this.subInventories[stack.GetItem().GetType()].TryAddItemStack(stack);
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