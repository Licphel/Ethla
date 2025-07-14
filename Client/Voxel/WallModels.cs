using System;
using Ethla.World;
using Ethla.World.Voxel;
using Spectrum.Graphic.Images;

namespace Ethla.Client.Voxel;

public class WallModels
{

    public static readonly Dictionary<int, WallRenderData> DataMap = new Dictionary<int, WallRenderData>();

    public static WallRenderer GetRenderer(Wall wall)
    {
        return DataMap[wall.Uid].Renderer;
    }

    public static Icon GetIcon(Wall wall)
    {
        return DataMap[wall.Uid].Icon;
    }

    public static Image GetMask(Wall wall)
    {
        return DataMap[wall.Uid].Mask;
    }

    public static bool IsConnectable(Wall inst, Wall other, Direction direction)
    {
        return other.GetShape().IsFull && inst.GetShape().IsFull;
    }

    public static bool IsSpreadable(Wall inst, Wall other, Direction direction)
    {
        return other != inst;
    }

    public class WallRenderData
    {

        public Icon Icon;
        public Image Mask;
        public WallRenderer Renderer;

	}

}
