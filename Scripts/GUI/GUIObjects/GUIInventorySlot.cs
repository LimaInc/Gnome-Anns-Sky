using System;
using Godot;

public class GUIInventorySlot : GUIObject
{
    public static readonly Texture TEX = ResourceLoader.Load(Game.GUI_TEXTURES_DIR_PATH + "inventorySlot.png") as Texture;
    public static readonly Vector2 SIZE = new Vector2(32, 32);

    private static readonly Vector2 HOVER_LABEL_OFFSET = SIZE / 2;

    private GUIInventorySlot exchangeSlot;
    private int index;
    private Item.ItemType type;

    private ItemStack stack;

    public const int CUR_ITEM_CHILD_Z = 1;
    private Sprite curItemChild;
    public const int LABEL_Z = 1;
    private GUILabel labelChild;
    private int labelNum;

    public const int HOVER_LABEL_Z = 3;
    private GUILabel hoverLabel;

    private Func<ItemStack,bool> quickMoveItem;
    private Action invUpdate;

    // TODO: use the invUpdate delegate idea to refactor the inventory with GUI synchronisation system
    public GUIInventorySlot(GUIInventorySlot exchangeSlot_, Item.ItemType type_, int index_, Vector2 pos, 
        Func<ItemStack,bool> quickMoveItem_ = null, Action invUpdate_ = null, Func<bool> shouldShow = null)
        : base(new Vector2(), SIZE, TEX, shouldShow) 
    {
        SetPosition(pos);

        exchangeSlot = exchangeSlot_;
        quickMoveItem = quickMoveItem_;
        invUpdate = invUpdate_;
        index = index_;
        type = type_;

        labelChild = new GUILabel();
        labelChild.SetZAsRelative(true);
        labelChild.SetZIndex(LABEL_Z);

        hoverLabel = new GUILabel();
        hoverLabel.Hide();
        hoverLabel.SetZAsRelative(true);
        hoverLabel.SetZIndex(HOVER_LABEL_Z);
        AddChild(hoverLabel);

        AssignItemStack(null);
    }

    public override void OnLeftPress()
    {
        ItemStack exchangeStack = exchangeSlot.GetCurItemStack();
        ItemStack slotStack = GetCurItemStack();

        if (quickMoveItem != null  && slotStack != null && Input.IsActionPressed("quick_move_items"))
        {
            if (quickMoveItem(slotStack))
            {
                ClearItemStack();
                invUpdate?.Invoke();
            }
            return;
        }

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
            this.AssignItemStack(exchangeStack);
        }
    }

    public override void OnHover()
    {
        hoverLabel.Show();
    }

    public override void OnHoverOff()
    {
        hoverLabel.Hide();
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
        if (stack == null)
        {
            AssignItemStack(i);
            return null;
        }
        else if(stack.Item == i.Item && stack.Item.Stackable)
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
        if (stack != null && labelNum != stack?.Count)
        {
            UpdateLabel();
        }
    }

    public override void _Input(InputEvent e)
    {
        if (e is InputEventMouseMotion iemm)
        {
            hoverLabel.SetPosition(ToLocal(iemm.GetPosition()) + HOVER_LABEL_OFFSET);
        }
    }

    private void UpdateCurrentItem()
    {
        if (curItemChild != null)
        {
            hoverLabel.Hide();
            curItemChild.RemoveChild(labelChild);
            RemoveChild(curItemChild);
            curItemChild = null;
        }
        if (stack != null)
        {
            curItemChild = stack.Item.GenerateGUISprite();
            curItemChild.Position = index != -1 ?
                rect.Position + SIZE / 2 :
                rect.Position;
            curItemChild.ZAsRelative = true;
            curItemChild.ZIndex = CUR_ITEM_CHILD_Z;
            AddChild(curItemChild);
            curItemChild.AddChild(labelChild);
            hoverLabel.Text = stack.Item.Name;
        }
        else
        {
            hoverLabel.Text = "";
        }
    }

    private void UpdateLabel()
    {
        labelNum = stack != null ? stack.Count : -1;
        Vector2 position = index != -1 ?
            rect.Position + SIZE / 2 :
            rect.Position;

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