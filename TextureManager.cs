using Godot;
using System;

public class TextureManager
{

    public ImageTexture AtlasTexture
    {
        get;
        private set;
    }

    public TextureManager()
    {
        CreateTextureAtlas("stone.png", "grass.png");
    }
    public TextureManager(params string[] paths)
    {
        CreateTextureAtlas(paths);
    }

    private void CreateTextureAtlas(params string[] paths)
    {
        Texture[] textures = new Texture[paths.Length];
        Image[] images = new Image[paths.Length];
        for(int i = 0; i < paths.Length; i++)
        {
            textures[i] = GD.Load("res://" + paths[i]) as Texture;
            if(textures[i] == null)
            {
                GD.Printerr("Failed to load texture at " + paths[i]);
            }
            else
            {
                images[i] = textures[i].GetData();
            }
        }

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

        AtlasTexture = new ImageTexture();
        AtlasTexture.CreateFromImage(atlasImage, 0);
    }
}