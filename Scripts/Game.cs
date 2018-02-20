using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Game : Node
{
    public const string GAME_PATH = "/root/Game";
    public const string WORLD_ENVIRO_PATH = GAME_PATH + "/WorldEnvironment";
    public const string TERRAIN_PATH = GAME_PATH + "/Terrain";
    public const string PLAYER_PATH = GAME_PATH + "/Player";
    public const string CAMERA_PATH = PLAYER_PATH+"/Camera";

    public ExoWorld World {get; private set;}

    public Game()
    {
        //Register blocks
        Game.RegisterBlock(new Stone());
        Game.RegisterBlock(new RedRock());
        Game.RegisterBlock(new OxygenBacteriaFossilBlock());
        Game.RegisterBlock(new NitrogenBacteriaFossilBlock());
        Game.RegisterBlock(new CarbonDioxideBacteriaFossilBlock());
        Game.RegisterBlock(new HabitationBlock());
        Game.RegisterBlock(new DefossiliserBlock());

        //Generate texture atlas once all blocks are registered
        GenerateTextureMap();

        World = new ExoWorld();
        AddChild(World);
    }

    public static Texture TextureAtlas { get; private set; }
    private static byte nextId = 1;
    private static Dictionary<byte, Block> blocks = new Dictionary<byte, Block>()
    {
        { 0, null } //Air block (probably should be something more sensible than null)
    };

    //UVs for all blocks
    private static Dictionary<byte, Rect2[]> blockUVs = new Dictionary<byte, Rect2[]>();

    public static Rect2 GetBlockUV(byte block, BlockFace face)
    {
        Rect2[] uvs = blockUVs[block];

        Rect2 uv = uvs[blocks[block].GetTextureIndex(face)];

        return uv;
    }

    //Generates texture atlas from all registered blocked
    private void GenerateTextureMap()
    {
        List<byte> texturedBlocks = blocks.Keys.Where(b => b != 0).ToList();
        List<Texture> textures = new List<Texture>();
        for(int i = 0; i < texturedBlocks.Count; i++)
        {
            string[] blockTexturePaths = blocks[texturedBlocks[i]].TexturePaths;
            Texture[] blockTextures = new Texture[blockTexturePaths.Length];

            for(int j = 0; j < blockTexturePaths.Length; j++)
            {
                blockTextures[j] = GD.Load(blockTexturePaths[j]) as Texture;
                if(blockTextures[j] == null)
                {
                    GD.Printerr("Block texture could not be loaded");
                }
                else
                {
                    textures.Add(blockTextures[j]);
                }
            }
        }

        TextureAtlas = TextureManager.PackTextures(textures.ToArray(), out Rect2[] uvs);

        int index = 0;

        for(int i = 0; i < texturedBlocks.Count; i++)
        {
            Rect2[] uvsArr = new Rect2[blocks[texturedBlocks[i]].TexturePaths.Length];
            for(int j = 0; j < uvsArr.Length; j++)
            {
                uvsArr[j] = uvs[index++];
            }
            blockUVs[texturedBlocks[i]] = uvsArr;
        }
    }

    public static void RegisterBlock(Block block)
    {
        blocks[nextId++] = block;
    }

    public static Block GetBlock(byte id)
    {
        return blocks[id];
    }

    public static byte GetBlockId<T>() where T : Block
    {
        //I want to rework this, probably using a bidirectional map, instead of a dictionary (use two dictionaries?)
        return blocks.Where(b => b.Key != 0).First(b => (typeof(T) == b.Value.GetType())).Key;
    }
}