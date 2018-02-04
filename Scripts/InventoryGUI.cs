using System;
using Godot;

public class InventoryGUI : Node
{
    private static IntVector2 SLOT_COUNT = new IntVector2(4,10);

    private static Vector2 SLOT_SPACING = new Vector2(2, 2);

    private static float SECTION_SPACING = 128.0f;

    private static Vector2 BOX_SIZE = new Vector2(800.0f, 400.0f);

    private Vector2 viewportSize;

    private GUIInventorySlot[] consumables = new GUIInventorySlot[40];
    private GUIInventorySlot[] fossils = new GUIInventorySlot[40];
    private GUIInventorySlot[] blocks = new GUIInventorySlot[40];

    private GUIBox box;

    public InventoryGUI(Vector2 vSize)
    {
        this.viewportSize = vSize;

        box = new GUIBox(new Rect2(viewportSize / 2,BOX_SIZE));
        this.AddChild(box);

        for (int x = 0; x < SLOT_COUNT.x; x++)
        {
            for (int y = 0; y < SLOT_COUNT.y; y++)
            {
                Vector2 pos = viewportSize / 2 + new Vector2(x, y) * (GUIInventorySlot.SIZE + SLOT_SPACING) + GUIInventorySlot.SIZE / 2.0f;

                Vector2 consPos = pos;
                Node cons = consumables[x + y * SLOT_COUNT.x] = new GUIInventorySlot(consPos);

                Vector2 fossPos = pos + new Vector2(SLOT_COUNT.x, 0.0f) * (GUIInventorySlot.SIZE + SLOT_SPACING) + new Vector2(SECTION_SPACING, 0.0f);
                Node foss = fossils[x + y * SLOT_COUNT.x] = new GUIInventorySlot(fossPos);

                Vector2 blocPos = pos + (new Vector2(SLOT_COUNT.x, 0.0f) * (GUIInventorySlot.SIZE + SLOT_SPACING) + new Vector2(SECTION_SPACING, 0.0f)) * 2.0f;
                Node bloc = blocks[x + y * SLOT_COUNT.x] = new GUIInventorySlot(blocPos);

                // this.AddChild(cons);
                // this.AddChild(foss);
                // this.AddChild(bloc);
            }
        }
    }
}