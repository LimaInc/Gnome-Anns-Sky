using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Game : Node
{
    public Game()
    {
        //Register blocks
        Game.RegisterBlock(new Stone());
        Game.RegisterBlock(new Grass());

        //Generate texture atlas once all blocks are registered
        GenerateTextureMap();
    }

    public static Texture TextureAtlas { get; private set; }
    private static byte nextId = 1;
    private static Dictionary<byte, Block> blocks = new Dictionary<byte, Block>()
    {
        { 0, null } //Air block (probably should be something more sensible than null)
    };

    //Generates texture atlas from all registered blocked
    private void GenerateTextureMap()
    {
        List<Block> texturedBlocks = blocks.Values.Where(b => b != null).ToList();
        Texture[] textures = new Texture[texturedBlocks.Count];
        for(int i = 0; i < texturedBlocks.Count; i++)
        {
            Texture t = GD.Load(texturedBlocks[i].TexturePath) as Texture;
            if(t == null)
            {
                GD.Printerr("Block texture could not be loaded");
            }
            else
            {
                textures[i] = t;
            }
        }

        Rect2[] uvs;
        TextureAtlas = TextureManager.PackTextures(textures.ToArray(), out uvs);

        for(int i = 0; i < texturedBlocks.Count; i++)
        {
            texturedBlocks[i].UVs = uvs[i];
        }
    }

    public static void RegisterBlock(Block block)
    {
        blocks[nextId++] = block;
    }

    public static Rect2 GetBlockUVs(byte id)
    {
        return blocks[id].UVs;
    }

    public static byte GetBlockId<T>() where T : Block
    {
        //I want to rework this, probably using a bidirectional map, instead of a dictionary (use two dictionaries?)
        return blocks.Where(b => b.Key != 0).First(b => (typeof(T) == b.Value.GetType())).Key;
    }
}