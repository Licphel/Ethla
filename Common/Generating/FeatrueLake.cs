using Ethla.World;
using Ethla.World.Generating;
using Ethla.World.Voxel;
using Spectrum.Maths;
using Spectrum.Maths.Random;

namespace Ethla.Common.Generating;

public class FeatureLake : Feature
{

	private readonly Func<Liquid> liquid;
	private readonly float spread;

	public FeatureLake(Func<Liquid> liquid, float spread, float clusters)
	{
		IsSurfacePlaced = true;
		TryTimesPerChunk = clusters;

		this.liquid = liquid;
		this.spread = spread;
	}

	public override bool IsPlacable(Level level, int x, int y, Seed seed)
	{
		return y < Chunk.YOfSea + Chunk.ScaleOfCrust / 2;
	}

	public override void Place(Level level, int x, int y, Seed seed)
	{
		for (float i = -spread * 2; i < spread * 2; i++)
			for (float j = -spread; j < spread; j++)
			{
				int x1 = Mathf.Round(x + i);
				int y1 = Mathf.Round(y + j);
				float d = Mathf.Sqrt(i * i + j * j);
				if ((x1 - x) * (x1 - x) + (y1 - y) * (y1 - y)
				- 8 * (y - (Chunk.YOfSea + Chunk.ScaleOfCrust / 2)) >= spread * spread * 2)
					continue;
				if (level.GetBlock(x1, y1).Is(Groups.Carvable) && IsPlacable(level, x, y, seed))
				{
					level.SetBlock(BlockState.Empty, x1, y1);
					level.SetLiquid(new LiquidStack(liquid(), (int)(Liquid.MaxAmount * 0.95f)), x1, y1);
				}
			}
	}

}
