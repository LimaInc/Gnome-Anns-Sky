# Contributing
## Guidelines for contributing
...

#### Adding new blocks
1. Create class which extends Block
2. Override required properties and methods  
  * `TexturePaths` should return a `string[]` of paths to the textures used by the block. For example:
  ```C#
  public override string[] TexturePaths { get { return new[] { "res://Images/grass_top.png", "res://Images/grass_side.png" }; } }
  ```
  * `GetTextureIndex` should return an integer index into the `TexturePaths` array, for the provided `BlockFace`. For example: 
  ```C#
  public override int GetTextureIndex(BlockFace face)
  {
    switch(face)
    {
      case BlockFace.Top:
      case BlockFace.Bottom:
        return 0;
      default:
        return 1;
    }
  }
  ```
3. Register the block in Game construction, with `Game.RegisterBlock(new MyNewBlockType())`
4. The block can now be used elsewhere in code. If you need it's `byte` id, you can use `Game.GetBlockId<BlockType>()`. Please bear in mind this is a comparitively expensive operation, so you should cache the result.  
  If you need to get a Block, given an id, use `Game.GetBlock(byte id)`
