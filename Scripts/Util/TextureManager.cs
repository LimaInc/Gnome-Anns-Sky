using Godot;
using System;
using System.Linq;

public abstract class TextureManager
{
    // Space between each texture in atlas
    public const int MARGIN = 2;

    public static Texture PackTextures(Texture[] textures, out Rect2[] uvs)
    {
        Image[] images = new Image[textures.Length];
        images = textures.Select(t => t.GetData()).ToArray(); //Select is like map in ML

        uvs = new Rect2[textures.Length];

        Image atlasImage = new Image();
        atlasImage.Create(images[0].GetWidth() * images.Length + (MARGIN * images.Length + 1), images[0].GetHeight(), false, Image.Format.Rgb8);
        atlasImage.Lock();
        int xPos = MARGIN / 2;
        // We will repeat the texture into the margins, so if there are any floating point inaccuracies in the UVs, they point into valid pixels of the atlas
        for(int i = 0; i < images.Length; i++)
        {
            uvs[i] = new Rect2((float)xPos / atlasImage.GetWidth(), 0, (float)images[0].GetWidth() / atlasImage.GetWidth(), 1);

            images[i].Lock();
            for(int x = -(MARGIN / 2); x < images[i].GetWidth() + (MARGIN / 2); x++)
            {
                for(int y = 0; y < images[i].GetHeight(); y++)
                {
                    atlasImage.SetPixel(xPos + x, y, images[i].GetPixel((x + images[i].GetWidth()) % images[i].GetWidth(), y));
                }
            }
            xPos += images[0].GetWidth() + MARGIN;
            images[i].Unlock();
        }
        atlasImage.Unlock();

        ImageTexture textureAtlas = new ImageTexture();
        textureAtlas.CreateFromImage(atlasImage, 0);
        
        return textureAtlas;
    }
}