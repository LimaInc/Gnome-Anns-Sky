using System;
using Godot;

public class GUIInventorySlot : GUIObject
{
    public static readonly Texture TEX = ResourceLoader.Load(Game.GUI_TEXTURE_PATH + "inventorySlot.png") as Texture;
    public static readonly Vector2 SIZE = new Vector2(32, 32);

    private static readonly Vector2 HOVER_LABEL_OFFSET = SIZE / 2;

    private GUIInventorySlot exchangeSlot;
    private int index;
    private Item.Type type;

    private ItemStack stack;

    public const int CUR_ITEM_CHILD_Z = 1;
    private Sprite curItemChild;
    public const int LABEL_Z = 1;
    private Label2D labelChild;
    private int labelNum;

    public const int HOVER_LABEL_Z = 3;
    private Label2D hoverLabel;

    public GUIInventorySlot(GUIInventorySlot toExchangeOnPress, Item.Type t, int ind, Vector2 pos) 
        : base(new Vector2(), SIZE, TEX) 
    {
        this.SetPosition(pos);

        this.exchangeSlot = toExchangeOnPress;
        this.index = ind;
        this.type = t;

        this.labelChild = new Label2D();
        this.labelChild.SetZAsRelative(true);
        this.labelChild.SetZIndex(LABEL_Z);

        this.hoverLabel = new Label2D();
        this.hoverLabel.Hide();
        this.hoverLabel.SetZAsRelative(true);
        this.hoverLabel.SetZIndex(HOVER_LABEL_Z);
        this.AddChild(hoverLabel);

        this.AssignItemStack(null);
    }

    public override void OnLeftPress()
    {
        ItemStack floatingStack = exchangeSlot.GetCurItemStack();
        ItemStack slotStack = this.GetCurItemStack();

        if (floatingStack != null &&
            !Item.CompatibleWith(floatingStack.GetItem().GetType(), this.GetItemType()))
        {
            return;
        }

        if (slotStack != null &&
            !Item.CompatibleWith(slotStack.GetItem().GetType(), exchangeSlot.GetItemType()))
        {
            return;
        }

        exchangeSlot.AssignItemStack(slotStack);
        this.AssignItemStack(floatingStack);
    }

    public override void OnHover()
    {
        this.hoverLabel.Show();
    }

    public override void OnHoverOff()
    {
        this.hoverLabel.Hide();
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
        if (i != null && !Item.CompatibleWith(i.GetItem().GetType(), this.type))
        {
            throw new ArgumentException("Cannot assign stack "+i+" of type "+i.GetItem().GetType()+
                " to slot "+this+" that accepts "+this.type);
        }

        stack = i;
        UpdateSlot();
    }

    public ItemStack OfferItemStack(ItemStack i)
    {
        if (stack.GetItem() == i.GetItem())
        {
            stack.AddToQuantity(i.GetCount());
            UpdateLabel();
            return null;
        }
        else
        {
            return i;
        }
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (stack != null && labelNum != stack.GetCount())
        {
            UpdateLabel();
        }
    }

    public override void _Input(InputEvent e)
    {
        if (e is InputEventMouseMotion iemm)
        {
            this.hoverLabel.SetPosition(this.ToLocal(iemm.GetPosition()) + HOVER_LABEL_OFFSET);
        }
    }

    private void UpdateCurrentItem()
    {
        if (curItemChild != null)
        {
            this.hoverLabel.Hide();
            this.curItemChild.RemoveChild(labelChild);
            this.RemoveChild(curItemChild);
            curItemChild = null;
        }
        if (stack != null)
        {
            curItemChild = stack.GetItem().GenerateGUISprite();
            curItemChild.Position = this.index != -1 ?
                this.rect.Position + SIZE / 2 :
                this.rect.Position;
            curItemChild.ZAsRelative = true;
            curItemChild.ZIndex = CUR_ITEM_CHILD_Z;
            this.AddChild(curItemChild);
            curItemChild.AddChild(labelChild);
            hoverLabel.Text = stack.GetItem().GetName();
        }
    }

    private void UpdateLabel()
    {
        labelNum = stack != null ? stack.GetCount() : -1;
        Vector2 position = this.index != -1 ?
            this.rect.Position + SIZE / 2 :
            this.rect.Position;

        if (labelNum <= 1)
        {
            labelChild.Hide();
        }
        else
        {
            labelChild.Text = "" + labelNum;
            labelChild.Show();
        }
    }

    private void UpdateSlot()
    {
        UpdateCurrentItem();
        UpdateLabel();
    }

    public void ClearItemStack()
    {
        stack = null;
        UpdateSlot();
    }
}