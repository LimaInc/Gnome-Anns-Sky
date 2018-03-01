using System;
using System.Collections.Generic;
using Godot;

public abstract class Block
{
    abstract public bool Breakable { get; }

    abstract public string[] TexturePaths { get; }
    
    abstract public int GetTextureIndex(BlockFace face);

    abstract public void AddPosXFace(ref SurfaceTool surfaceTool, Vector3 origin, byte blockType);
    abstract public void AddNegXFace(ref SurfaceTool surfaceTool, Vector3 origin, byte blockType);
    abstract public void AddPosYFace(ref SurfaceTool surfaceTool, Vector3 origin, byte blockType);
    abstract public void AddNegYFace(ref SurfaceTool surfaceTool, Vector3 origin, byte blockType);
    abstract public void AddPosZFace(ref SurfaceTool surfaceTool, Vector3 origin, byte blockType);
    abstract public void AddNegZFace(ref SurfaceTool surfaceTool, Vector3 origin, byte blockType);

}