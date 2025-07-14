using System;
using Ethla.Common;

namespace Ethla.World.Voxel;

public class WallProperty
{

    public bool CanBeReplace;
    public float Hardness = 0.0f;
    public float[] Light = [0, 0, 0];
    public BlockShape Shape = BlockShape.Solid;
    public BlockMaterial Material = BlockMaterials.Unknown;
    public int Rarity;

}
