using System;
using System.Collections.Generic;
using Godot;

public class DefossiliserGUI : GUI
{
    public static readonly IntVector2 IN_SLOT_COUNT = new IntVector2(2, 3);
    public static readonly IntVector2 OUT_SLOT_COUNT = new IntVector2(2, 3);

    private static readonly Vector2 BOX_SIZE = new Vector2(550, 400);
    private static readonly Vector2 SLOT_SPACING = new Vector2(2, 2);

    private static readonly Vector2 SLOT_OFFSET = new Vector2(20, 30);

    private GUIBox box;

    public DefossiliserGUI(Defossiliser d, InventoryGUI inventory, Node vdSource) : base(vdSource)
    {
        throw new NotImplementedException();
        /*
        box = new GUIBox(new Rect2(this.GetViewportDimensions() / 2, BOX_SIZE));
        this.AddChild(box);

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
        */
    }

    public override void HandleResize()
    {
        throw new NotImplementedException();
        /*{
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

        box = new GUIBox(new Rect2(this.GetViewportDimensions() / 2, BOX_SIZE));
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
        this.AddChild(floatingSlot);*/
    }
}