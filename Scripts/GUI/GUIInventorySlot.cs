using System;
using Godot;

public class GUIInventorySlot : GUIObject
{
    public static Texture TEX = ResourceLoader.Load("res://Images/inventorySlot.png") as Texture;
    public static Vector2 SIZE = new Vector2(32.0f, 32.0f);

    private InventoryGUI inventory;
    private int index;
    private Item.Type type;

    private ItemStack stack;

    private Sprite curItemChild;
    private Label curLabelChild;

    public GUIInventorySlot(InventoryGUI inv, Item.Type t, int ind, Vector2 pos) : base(new Rect2(pos, SIZE), TEX) 
    { 
        this.inventory = inv;
        this.index = ind;
        this.type = t;
    }

    //For floating slot use ONLY
    public GUIInventorySlot() : base(new Rect2(new Vector2(), SIZE), new ImageTexture()) 
    { 
        this.index = -1;
    }

    public override void onClick()
    {
        if (this.index != -1) //If i am not the floating slot
            inventory.HandleSlotClick(index, type);
    }

    public void SetPosition(Vector2 pos)
    {
        this.rect.Position = pos;
        this.sprite.SetPosition(pos);

        if (this.curItemChild != null)
        {
            this.curItemChild.SetPosition(pos);

            if (this.stack.GetCount() > 1)
                this.curLabelChild.SetPosition(pos);
        }
    }

    public Item.Type GetItemType()
    {
        return this.type;
    }

    public ItemStack GetCurItemStack()
    {
        return stack;
    }

    public void AssignItemStack(ItemStack i)
    {
        if (i == null) 
            return;

        stack = i;
        curItemChild = i.GetItem().GenerateGUISprite();

        if (this.index != -1)
            curItemChild.SetPosition(this.rect.Position + GUIInventorySlot.SIZE / 2.0f);
        else
            curItemChild.SetPosition(this.rect.Position);

        this.AddChild(curItemChild);

        if (i.GetCount()  > 1)
        {
            curLabelChild = new Label();
            curLabelChild.SetText("" + i.GetCount());

            if (this.index != -1)
                curLabelChild.SetPosition(this.rect.Position + GUIInventorySlot.SIZE / 2.0f);
            else
                curLabelChild.SetPosition(this.rect.Position);

            this.AddChild(curLabelChild);
        }
    }

    public void ClearItemStack()
    {
        stack = null;

        if (curLabelChild != null)
            this.RemoveChild(curLabelChild);
            
        this.RemoveChild(curItemChild);
    }
}