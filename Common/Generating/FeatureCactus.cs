using System;
using Ethla.World;
using Ethla.World.Generating;
using Ethla.World.Voxel;
using Spectrum.Maths.Random;

namespace Ethla.Common.Generating;

public class FeatureCactus : Feature
{

    public FeatureCactus()
	{
		IsSurfacePlaced = true;
		TryTimesPerChunk = 2;
	}

	public override bool IsPlacable(Level level, int x, int y, Seed seed)
	{
		return level.GetBlock(x, y).Is(Groups.Sand);
	}

	public override void Place(Level level, int x, int y, Seed seed)
	{
		BlockState state1 = Blocks.Cactus.MakeState(0);
		BlockState state2 = Blocks.Cactus.MakeState(1);
		BlockState state3 = Blocks.Cactus.MakeState(2);

		int h = seed.NextInt(2, 5);

		for (int i = 0; i < h; i++)
		{
			if (i == h - 1)
				level.SetBlock(seed.Next() ? state2 : state3, x, y + i + 1);
			else
				level.SetBlock(state1, x, y + i + 1);
		}
	}

}
