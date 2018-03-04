using System;

public class BaseGenerator : IGenerator
{
    private readonly byte HAB_ID = Game.GetBlockId<HabitationBlock>();
    private readonly byte DEFOSSILISER_ID = Game.GetBlockId<DefossiliserBlock>();
    private const byte AIR_ID = WorldGenerator.AIR_ID;

    private Base @base;
    
    private readonly float radiusSquared;

    public readonly IntVector3 defossiliserLocalPosition;

    public BaseGenerator(Base b)
    {
        @base = b;

        radiusSquared = @base.radius * @base.radius;

        defossiliserLocalPosition = new IntVector3(0, 1, 2 - (int)@base.radius);
    }

    public void GenerateChunk(byte[,,] chunk, int[,] chunkTerrainHeight, IntVector2 chunkIndex, IntVector3 chunkSize)
    {
        for (int i = 0; i < chunkSize.x; i++)
        {
            for (int k = 0; k < chunkSize.z; k++)
            {
                IntVector2 worldCoords = new IntVector2(chunkIndex.x * chunkSize.x + i, chunkIndex.y * chunkSize.z + k);
                IntVector2 localCoords = worldCoords - new IntVector2(@base.position.x, @base.position.z);

                if (localCoords.LengthSquared() < radiusSquared && localCoords.x < @base.radius - @base.baseEntranceDepth)
                {
                    for (int baseY = 0; baseY <= @base.radius - @base.domeOffset; baseY++)
                    {
                        int j = baseY + @base.position.y;
                        IntVector3 local3DCoords = new IntVector3(localCoords.x, baseY, localCoords.y);
                        float domeDistance = (local3DCoords + new IntVector3(0, @base.domeOffset, 0)).Length();
                        // prepare space for base
                        if (domeDistance < @base.radius)
                        {
                            chunk[i, j, k] = AIR_ID;
                        }
                        // generate base walls (floor and dome) 
                        if (baseY == 0 || Math.Abs(domeDistance - @base.radius) < 0.5)
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
