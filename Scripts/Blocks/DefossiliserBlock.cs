using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class DefossiliserBlock : CubeBlock
{
    private static readonly string[] texturePaths = 
        new[] { "res://Images/defossiliser_front.png",
                "res://Images/defossiliser_top.png",
                "res://Images/defossiliser_side.png" };

    public override string[] TexturePaths { get => texturePaths; }

    public override int GetTextureIndex(BlockFace face)
    {
        switch (face)
        {
            case BlockFace.Front:
                return 0;
            case BlockFace.Top:
                return 1;
            default:
                return 2;
        }
    }
}
