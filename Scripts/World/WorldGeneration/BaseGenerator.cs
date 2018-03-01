using System;

public class BaseGenerator : IGenerator
{
    private readonly byte HAB_ID = Game.GetBlockId<HabitationBlock>();
    private readonly byte DEFOSSILISER_ID = Game.GetBlockId<DefossiliserBlock>();
    private const byte AIR_ID = WorldGenerator.AIR_ID;

    private Base @base;

    public readonly float radiusSquared;

    public readonly IntVector3 defossiliserLocalPosition;

    public BaseGenerator(Base b)
    {
        @base = b;

        this.radiusSquared = @base.radius * @base.radius;

        this.defossiliserLocalPosition = new IntVector3(0, @base.baseFloorHeight + 1, 2 - (int)@base.radius);
    }

    public void GenerateChunk(byte[,,] chunk, int[,] chunkTerrainHeight, IntVector2 chunkIndex, IntVector3 chunkSize)
    {
        for (int i = 0; i < chunkSize.x; i++)
        {
            for (int k = 0; k < chunkSize.z; k++)
            {
                IntVector2 worldCoords = new IntVector2(chunkIndex.x * chunkSize.x + i, chunkIndex.y * chunkSize.z + k);
                IntVector2 localCoords = worldCoords - new IntVector2(@base.position.x, @base.position.z);

                for (int baseY = @base.baseFloorHeight; baseY <= @base.radius; baseY++)
                {
                    int j = baseY + @base.position.y;

                    if (localCoords.LengthSquared() < radiusSquared && localCoords.x < @base.radius - @base.baseEntranceDepth)
                    {
                        IntVector3 local3DCoords = new IntVector3(localCoords.x, baseY, localCoords.y);
                        float blockSphereDist = local3DCoords.Length();
                        // prepare space for base
                        if (blockSphereDist < @base.radius)
                        {
                            chunk[i, j, k] = AIR_ID;
                        }
                        // generate base walls (floor and dome) 
                        if (baseY == @base.baseFloorHeight || Math.Abs(blockSphereDist - @base.radius) < 0.5)
                        {
                            chunk[i, j, k] = HAB_ID;
                        }
                        // generate defossiliser
                        if (local3DCoords == defossiliserLocalPosition)
                        {
                            chunk[i, j, k] = DEFOSSILISER_ID;
                        }
                    }
                }
            }
        }
    }
}
