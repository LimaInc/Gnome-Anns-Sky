using System;
using Godot;

public class InventoryGUI : GUI
{
    private static IntVector2 SLOT_COUNT = new IntVector2(4,10);

    private static Vector2 SLOT_SPACING = new Vector2(2, 2);

    private static Vector2 SLOT_OFFSET = new Vector2(20.0f, 30.0f);

    private static Vector2 BOX_SIZE = new Vector2(550.0f, 400.0f);

    private Inventory consumableInventory;
    private Inventory fossilInventory;
    private Inventory blockInventory;

    //This follows the mouse to allow the player to move items around
    private GUIInventorySlot floatingSlot;

    private GUIInventorySlot[] consSlots = new GUIInventorySlot[40];
    private GUIInventorySlot[] fossSlots = new GUIInventorySlot[40];
    private GUIInventorySlot[] blocSlots = new GUIInventorySlot[40];

    private GUIInventorySlot handSlot;

    private Label consLabel;
    private Label fossLabel;
    private Label blocLabel;

    private GUIBox box;

    private Label hoverLabel;

    private Player player;

    public InventoryGUI(Player player, Inventory cinv, Inventory finv, Inventory binv, Node vSource) : base(vSource)
    {
        this.player = player;
        this.consumableInventory = cinv;
        this.fossilInventory = finv;
        this.blockInventory = binv;
    }

    private bool first = true;

    public override void HandleResize()
    {
        if (!first)
        {
            this.RemoveChild(box);
            this.RemoveChild(consLabel);
            this.RemoveChild(fossLabel);
            this.RemoveChild(blocLabel);

            foreach (GUIInventorySlot g in consSlots)
                this.RemoveChild(g);

            foreach (GUIInventorySlot g in fossSlots)
                this.RemoveChild(g);

            foreach (GUIInventorySlot g in blocSlots)
                this.RemoveChild(g);

            this.RemoveChild(floatingSlot);
            this.RemoveChild(handSlot);
        }

        box = new GUIBox(new Rect2(this.GetViewportDimensions() / 2,BOX_SIZE));
        this.AddChild(box);

        handSlot = new GUIInventorySlot(this, Item.Type.BLOCK, -2, this.GetViewportDimensions() / 2.0f + new Vector2(-16.0f, 170.0f));
        handSlot.AssignItemStack(player.ItemInHand);
        this.AddChild(handSlot);

        Vector2 sectionSize = new Vector2(SLOT_COUNT.x, SLOT_COUNT.y) * (SLOT_SPACING + GUIInventorySlot.SIZE);
        Vector2 sideSpace = SLOT_OFFSET * 2.0f;
        float sectionSpacing = (BOX_SIZE.x - 3 * sectionSize.x) / 2.0f - SLOT_OFFSET.x;

        Vector2 totOff = SLOT_OFFSET + this.GetViewportDimensions() / 2 - BOX_SIZE / 2.0f;

        for (int x = 0; x < SLOT_COUNT.x; x++)
        {
            for (int y = 0; y < SLOT_COUNT.y; y++)
            {
                Vector2 pos = totOff + new Vector2(x, y) * (GUIInventorySlot.SIZE + SLOT_SPACING) + GUIInventorySlot.SIZE / 2.0f;

                int ind = x + y * SLOT_COUNT.x;

                Vector2 consPos = pos - GUIInventorySlot.SIZE / 2.0f;
                GUIInventorySlot cons = consSlots[ind] = new GUIInventorySlot(this, Item.Type.CONSUMABLE, ind, consPos);
                cons.AssignItemStack(consumableInventory.GetItemStack(ind));

                Vector2 fossPos = consPos + new Vector2(sectionSize.x + sectionSpacing, 0.0f);
                GUIInventorySlot foss = fossSlots[ind] = new GUIInventorySlot(this, Item.Type.FOSSIL, ind, fossPos);
                foss.AssignItemStack(fossilInventory.GetItemStack(ind));

                Vector2 blocPos = consPos + (new Vector2(sectionSize.x + sectionSpacing, 0.0f)) * 2.0f;
                GUIInventorySlot bloc = blocSlots[ind] = new GUIInventorySlot(this, Item.Type.BLOCK, ind, blocPos);
                bloc.AssignItemStack(blockInventory.GetItemStack(ind));

                this.AddChild(cons);
                this.AddChild(foss);
                this.AddChild(bloc);
            }
        }

        consLabel = new Label();
        consLabel.SetText("Consumables");
        consLabel.SetPosition(totOff + new Vector2(sectionSize.x + sectionSpacing, 0.0f) * 0 - new Vector2(0.0f, 16.0f));
        this.AddChild(consLabel);

        fossLabel = new Label();
        fossLabel.SetText("Fossils");
        fossLabel.SetPosition(totOff + new Vector2(sectionSize.x + sectionSpacing, 0.0f) * 1 - new Vector2(0.0f, 16.0f));
        this.AddChild(fossLabel);

        blocLabel = new Label();
        blocLabel.SetText("Blocks");
        blocLabel.SetPosition(totOff + new Vector2(sectionSize.x + sectionSpacing, 0.0f) * 2 - new Vector2(0.0f, 16.0f));
        this.AddChild(blocLabel);

        floatingSlot = new GUIInventorySlot();
        this.AddChild(floatingSlot);

        first = false;
    }

    public void HandleHover(ItemStack ist)
    {
        if (ist == null || hoverLabel != null)
            return;

        this.hoverLabel = new Label();
        this.hoverLabel.SetText(ist.GetItem().GetName());
        this.AddChild(this.hoverLabel);
    }

    public void HandleHoverOff()
    {
        if (this.hoverLabel == null)
            return;

        this.RemoveChild(this.hoverLabel);
        this.hoverLabel = null;
    }

    public void HandleClose()
    {
        ItemStack i = floatingSlot.GetCurItemStack();
        if (i != null)
        {
            Item.Type type = i.GetItem().GetType();
            if (type == Item.Type.CONSUMABLE)
                consumableInventory.AddItemStack(i);
            if (type == Item.Type.FOSSIL)
                fossilInventory.AddItemStack(i);
            if (type == Item.Type.BLOCK)
                blockInventory.AddItemStack(i);
        }
    }

    public void HandleSlotClick(int index, Item.Type type)
    {
        ItemStack floatingItemStack = floatingSlot.GetCurItemStack();

        //Hand slot
        if (index == -2)
        {
            ItemStack ist = handSlot.GetCurItemStack();

            if (ist == null && floatingItemStack == null)
                return;

            if (floatingItemStack == null)
            {
                floatingSlot.AssignItemStack(ist);
                handSlot.ClearItemStack();
                player.ItemInHand = null;
                return;
            }

            if (ist == null)
            {
                handSlot.AssignItemStack(floatingItemStack);
                player.ItemInHand = floatingItemStack;
                floatingSlot.ClearItemStack();
                return;
            }

            handSlot.ClearItemStack();
            handSlot.AssignItemStack(floatingItemStack);
            player.ItemInHand = floatingItemStack;
            floatingSlot.ClearItemStack();
            floatingSlot.AssignItemStack(ist);

            return;
        }

        GUIInventorySlot slot = null;

        if (type == Item.Type.CONSUMABLE)
        {
            slot = consSlots[index];
        }
        if (type == Item.Type.FOSSIL)
        {
            slot = fossSlots[index];
        }
        if (type == Item.Type.BLOCK)
        {
            slot = blocSlots[index];
        }

        ItemStack iStack = slot.GetCurItemStack();

        if (iStack == null && floatingItemStack == null)
            return;

        if (floatingItemStack == null)
        {
            Item.Type iiType = iStack.GetItem().GetType();

            floatingSlot.AssignItemStack(iStack);
            slot.ClearItemStack();
            
            if (iiType == Item.Type.CONSUMABLE)
            {
                consumableInventory.RemoveItemStack(index);
            }
            if (iiType == Item.Type.FOSSIL)
            {
                fossilInventory.RemoveItemStack(index);
            }
            if (iiType == Item.Type.BLOCK)
            {
                blockInventory.RemoveItemStack(index);
            }

            return;
        }

        if (iStack == null)
        {
            Item.Type ffType = floatingItemStack.GetItem().GetType();

            if (ffType != slot.GetItemType())
                return;
            
            if (ffType == Item.Type.CONSUMABLE)
            {
                consumableInventory.AddItemStack(floatingItemStack, index);
            }
            if (ffType == Item.Type.FOSSIL)
            {
                fossilInventory.AddItemStack(floatingItemStack, index);
            }
            if (ffType == Item.Type.BLOCK)
            {
                blockInventory.AddItemStack(floatingItemStack, index);
            }

            slot.AssignItemStack(floatingItemStack);
            floatingSlot.ClearItemStack();
            return;
        }
        
        Item.Type iType = iStack.GetItem().GetType();
        Item.Type fType = floatingItemStack.GetItem().GetType();

        if (fType != iType)
            return;

        //Both slots contain something
        slot.ClearItemStack();
        floatingSlot.ClearItemStack();

        slot.AssignItemStack(floatingItemStack);
        floatingSlot.AssignItemStack(iStack);
            
        if (iType == Item.Type.CONSUMABLE)
        {
            consumableInventory.RemoveItemStack(index);
            consumableInventory.AddItemStack(floatingItemStack, index);
        }
        if (iType == Item.Type.FOSSIL)
        {
            fossilInventory.RemoveItemStack(index);
            fossilInventory.AddItemStack(floatingItemStack, index);
        }
        if (iType == Item.Type.BLOCK)
        {
            blockInventory.RemoveItemStack(index);
            blockInventory.AddItemStack(floatingItemStack, index);
        }
    }

    public override void _Input(InputEvent e)
    {
        if (e is InputEventMouseMotion iemm)
        {
            floatingSlot.SetPosition(iemm.GetPosition());

            if (this.hoverLabel != null)
                this.hoverLabel.SetPosition(iemm.GetPosition() + new Vector2(10.0f, 10.0f));
        }
    }
}