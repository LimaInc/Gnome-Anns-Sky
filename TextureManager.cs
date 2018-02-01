using Godot;
using System;
using System.Linq;

public abstract class TextureManager
{
    public static Texture PackTextures(Texture[] textures, out Rect2[] uvs)
    {
        Image[] images = new Image[textures.Length];
        images = textures.Select(t => t.GetData()).ToArray(); //Select is like map in ML

        Image atlasImage = new Image();
        atlasImage.Create(images[0].GetWidth() * images.Length, images[0].GetHeight(), false, Image.Format.Rgb8);
        atlasImage.Lock();
        for(int i = 0; i < images.Length; i++)
        {
            images[i].Lock();
            for(int x = 0; x < images[i].GetWidth(); x++)
            {
                for(int y = 0; y < images[i].GetHeight(); y++)
                {
                    atlasImage.SetPixel((images[0].GetWidth() * i) + x, y, images[i].GetPixel(x, y));
                }
            }
            images[i].Unlock();
        }
        atlasImage.Unlock();

        ImageTexture textureAtlas = new ImageTexture();
        textureAtlas.CreateFromImage(atlasImage, 0);
        
        uvs = null;

        return textureAtlas;
    }
}