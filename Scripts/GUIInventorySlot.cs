using System;
using Godot;

public class GuiInventorySlot : GUIObject
{
        // //Generate texture atlas
        // Texture[] textures = { ResourceLoader.Load("res://Images/stone.png") as Texture,
        //                        ResourceLoader.Load("res://Images/grass.png") as Texture
        // };
        // textureAtlas = TextureManager.PackTextures(textures, out textureUVs);
    public GuiInventorySlot(Rect2 r) : base(r)
    {
        
    }
}