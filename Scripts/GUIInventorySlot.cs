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

    public GUIInventorySlot(Vector2 pos) : base(new Rect2(pos, SIZE), TEX) { }
}