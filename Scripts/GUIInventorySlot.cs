using System;
using Godot;

public class GUIInventorySlot : GUIObject
{

        // //Generate texture atlas
        // Texture[] textures = { ResourceLoader.Load("res://Images/stone.png") as Texture,
        //                        ResourceLoader.Load("res://Images/grass.png") as Texture
        // };
        // textureAtlas = TextureManager.PackTextures(textures, out textureUVs);

    public static Texture TEX = ResourceLoader.Load("res://Images/inventorySlot.png") as Texture;
    public static Vector2 SIZE = new Vector2(32.0f, 32.0f);

    private InventoryGUI inventory;
    private int index;
    private Item.Type type;

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

    private Sprite curItemChild; //if any
    private Item curItem; //if any

    public void SetPosition(Vector2 pos)
    {
        this.rect.Position = pos;
        this.sprite.SetPosition(pos);

        if (this.curItemChild != null)
        {
            this.curItemChild.SetPosition(pos);
        }
    }

    public Item GetCurItem()
    {
        return curItem;
    }

    public void AssignItem(Item i)
    {
        if (i == null) 
            return;

        curItem = i;
        curItemChild = i.generateGUISprite();

        if (this.index != -1)
            curItemChild.SetPosition(this.rect.Position + GUIInventorySlot.SIZE / 2.0f);
        else
            this.curItemChild.SetPosition(this.rect.Position);
            
        this.AddChild(curItemChild);
    }

    public void ClearItem()
    {
        if (curItemChild == null)
            return;

        curItem = null;
        this.RemoveChild(curItemChild);
    }
}