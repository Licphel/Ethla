using Ethla.World.Voxel;
using Spectrum.Core.Manage;
using Spectrum.Graphic.Images;

namespace Ethla.Client.Voxel;

public class LiquidModels
{

	public static Image GetImage(Liquid liquid)
	{
		return Loads.Get($"{liquid.Uid.Space}:image/liquid/{liquid.Uid.Key}.png");
	}

	public static Image GetImageEdge(Liquid liquid)
	{
		return Loads.Get($"{liquid.Uid.Space}:image/liquid/{liquid.Uid.Key}_edge.png");
	}

}
