using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Game : Node
{
    public const string GAME_PATH = "/root/Game";
    public const string TERRAIN_PATH = GAME_PATH + "/Terrain";
    public const string PLAYER_PATH = GAME_PATH + "/Player";
    public const string CAMERA_PATH = PLAYER_PATH+"/Camera";
    public const string PLANET_BASE_PATH = GAME_PATH + "/Base";
    public const string WORLD_ENVIRO_PATH = GAME_PATH + "/PlanetEnvironment";
    public const string PLANTS_PATH = WORLD_ENVIRO_PATH + "/Plants";
    public const string ATMOSPHERE_PATH = WORLD_ENVIRO_PATH + "/Atmosphere";
    public const string BACTERIAL_STATE_PATH = WORLD_ENVIRO_PATH + "/BacterialState";
    public const string ANIMAL_SPAWNER_PATH = GAME_PATH + "/AnimalSpawner";

    public const string GUI_TEXTURE_PATH = "res://Images/GUI/";
    public const string BLOCK_TEXTURE_PATH = "res://Images/Blocks/";
    public const string ITEM_TEXTURE_PATH = "res://Images/Items/";

    // multiplicative factor for processes in the world (not directly affecting the player)
    public const int FOSSIL_SPAWN_MULITPLIER = 30;
    public const int SPEED = 30;
    public const int PLANT_MAX_SPEED = 2; // if plants are spreading too fast bugs happen, this should NOT be a feature, TODO: fix

    public Game()
    {
        //Register blocks
        RegisterBlock(new Stone());
        RegisterBlock(new RedRock());

        RegisterBlock(new OxygenBacteriaFossilBlock());
        RegisterBlock(new NitrogenBacteriaFossilBlock());
        RegisterBlock(new CarbonDioxideBacteriaFossilBlock());
        RegisterBlock(new GrassFossilBlock());
        RegisterBlock(new TreeFossilBlock());
        RegisterBlock(new FrogFossilBlock());
        RegisterBlock(new RegularAnimalFossilBlock());
        RegisterBlock(new BigAnimalFossilBlock());

        RegisterBlock(new GrassBlock());
        RegisterBlock(new TreeBlock());
        RegisterBlock(new LeafBlock());
        RegisterBlock(new IceBlock());

        RegisterBlock(new HabitationBlock());
        RegisterBlock(new DefossiliserBlock());

        //Generate texture atlas once all blocks are registered
        GenerateTextureMap();
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
