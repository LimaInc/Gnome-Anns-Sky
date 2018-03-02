using System;
using Godot;

public class GUIInventorySlot : GUIObject
{
    public static readonly Texture TEX = ResourceLoader.Load(Game.GUI_TEXTURE_PATH + "inventorySlot.png") as Texture;
    public static readonly Vector2 SIZE = new Vector2(32, 32);

    private static readonly Vector2 HOVER_LABEL_OFFSET = SIZE / 2;

    private GUIInventorySlot exchangeSlot;
    private int index;
    private Item.ItemType type;

    private ItemStack stack;

    public const int CUR_ITEM_CHILD_Z = 1;
    private Sprite curItemChild;
    public const int LABEL_Z = 1;
    private Label2D labelChild;
    private int labelNum;

    public const int HOVER_LABEL_Z = 3;
    private Label2D hoverLabel;

    public GUIInventorySlot(GUIInventorySlot toExchangeOnPress, Item.ItemType t, int ind, Vector2 pos) 
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
        ItemStack exchangeStack = exchangeSlot.GetCurItemStack();
        ItemStack slotStack = GetCurItemStack();

        if (exchangeStack != null &&
            !Item.CompatibleWith(exchangeStack.Item.IType, this.GetItemType()))
        {
            return;
        }

        if (slotStack != null &&
            !Item.CompatibleWith(slotStack.Item.IType, exchangeSlot.GetItemType()))
        {
            return;
        }

        // TODO: think of a better conditional
        if (exchangeStack != null && slotStack != null && slotStack.Item.Stackable && slotStack.Item.Id == exchangeStack.Item.Id)
        {
            slotStack.ChangeQuantity(exchangeStack.Count);
            exchangeSlot.ClearItemStack();
        }
        else
        {
            exchangeSlot.AssignItemStack(slotStack);
            AssignItemStack(exchangeStack);
        }
    }

    public override void OnHover()
    {
        this.hoverLabel.Show();
    }

    public override void OnHoverOff()
    {
        this.hoverLabel.Hide();
    }

    public Item.ItemType GetItemType()
    {
        return this.type;
    }

    public ItemStack GetCurItemStack()
    {
        return stack;
    }

    public void AssignItemStack(ItemStack i)
    {
        if (i != null && !Item.CompatibleWith(i.Item.IType, this.type))
        {
            throw new ArgumentException("Cannot assign stack "+i+" of type "+i.Item.GetType()+
                " to slot "+this+" that accepts "+this.type);
        }

        stack = i;
        UpdateSlot();
    }

    public ItemStack OfferItemStack(ItemStack i)
    {
        if (stack.Item == i.Item)
        {
            stack.ChangeQuantity(i.Count);
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

        if (stack != null && labelNum != stack.Count)
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
            curItemChild = stack.Item.GenerateGUISprite();
            curItemChild.Position = this.index != -1 ?
                this.rect.Position + SIZE / 2 :
                this.rect.Position;
            curItemChild.ZAsRelative = true;
            curItemChild.ZIndex = CUR_ITEM_CHILD_Z;
            this.AddChild(curItemChild);
            curItemChild.AddChild(labelChild);
            hoverLabel.Text = stack.Item.Name;
        }
    }

    private void UpdateLabel()
    {
        labelNum = stack != null ? stack.Count : -1;
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