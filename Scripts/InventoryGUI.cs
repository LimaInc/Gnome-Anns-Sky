using System;
using Godot;

public class InventoryGUI : Node
{
    private static IntVector2 SLOT_COUNT = new IntVector2(4,10);

    private static Vector2 SLOT_SPACING = new Vector2(2, 2);

    private static Vector2 SLOT_OFFSET = new Vector2(20.0f, 30.0f);

    private static Vector2 BOX_SIZE = new Vector2(550.0f, 400.0f);

    private Vector2 viewportSize;

    //This follows the mouse to allow the player to move items around
    private GUIInventorySlot floatingSlot;

    private GUIInventorySlot[] consSlots = new GUIInventorySlot[40];
    private GUIInventorySlot[] fossSlots = new GUIInventorySlot[40];
    private GUIInventorySlot[] blocSlots = new GUIInventorySlot[40];

    private Label consLabel;
    private Label fossLabel;
    private Label blocLabel;

    private GUIBox box;

    public InventoryGUI(Vector2 vSize)
    {
        this.viewportSize = vSize;

        box = new GUIBox(new Rect2(viewportSize / 2,BOX_SIZE));
        this.AddChild(box);

        Vector2 sectionSize = new Vector2(SLOT_COUNT.x, SLOT_COUNT.y) * (SLOT_SPACING + GUIInventorySlot.SIZE);
        Vector2 sideSpace = SLOT_OFFSET * 2.0f;
        float sectionSpacing = (BOX_SIZE.x - 3 * sectionSize.x) / 2.0f - SLOT_OFFSET.x;

        Vector2 totOff = SLOT_OFFSET + viewportSize / 2 - BOX_SIZE / 2.0f;

        for (int x = 0; x < SLOT_COUNT.x; x++)
        {
            for (int y = 0; y < SLOT_COUNT.y; y++)
            {
                Vector2 pos = totOff + new Vector2(x, y) * (GUIInventorySlot.SIZE + SLOT_SPACING) + GUIInventorySlot.SIZE / 2.0f;

                Vector2 consPos = pos;
                Node cons = consSlots[x + y * SLOT_COUNT.x] = new GUIInventorySlot(consPos);

                Vector2 fossPos = pos + new Vector2(sectionSize.x + sectionSpacing, 0.0f);
                Node foss = fossSlots[x + y * SLOT_COUNT.x] = new GUIInventorySlot(fossPos);

                Vector2 blocPos = pos + (new Vector2(sectionSize.x + sectionSpacing, 0.0f)) * 2.0f;
                Node bloc = blocSlots[x + y * SLOT_COUNT.x] = new GUIInventorySlot(blocPos);

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
    }

    public override void _Input(InputEvent e)
    {
        if (e is InputEventMouseMotion)
        {
            InputEventMouseMotion iemm = (InputEventMouseMotion) e;

            floatingSlot.SetPosition(iemm.GetPosition());
        }
    }
}