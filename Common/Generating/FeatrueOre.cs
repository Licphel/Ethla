using Ethla.World;
using Ethla.World.Generating;
using Ethla.World.Voxel;
using Spectrum.Maths;
using Spectrum.Maths.Random;

namespace Ethla.Common.Generating;

public class FeatureOre : Feature
{

	private readonly float spread;
	private readonly Func<BlockState> state;

	public FeatureOre(float spread, float clusters, Func<BlockState> state)
	{
		IsSurfacePlaced = false;
		TryTimesPerChunk = clusters;
		this.spread = spread;
		this.state = state;
	}

	public override bool IsPlacable(Level level, int x, int y, Seed seed)
	{
		BlockState state = level.GetBlock(x, y);
		return state.Is(Groups.Rock);
	}

	public override void Place(Level level, int x, int y, Seed seed)
	{
		for (float i = -spread; i < spread; i++)
			for (float j = -spread; j < spread; j++)
			{
				int x1 = Mathf.Round(x + i);
				int y1 = Mathf.Round(y + j);
				float d = Mathf.Sqrt(i * i + j * j);
				float c = 1;
				if (d > spread / 2f) c = (spread - d) * 0.1f + 0.5f;
				if ((c == 1 || seed.NextFloat() < c) && IsPlacable(level, x1, y1, seed)) level.SetBlock(state(), x1, y1);
			}
	}

}
