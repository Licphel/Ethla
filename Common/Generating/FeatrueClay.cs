using Ethla.World;
using Ethla.World.Generating;
using Ethla.World.Voxel;
using Spectrum.Maths;
using Spectrum.Maths.Random;

namespace Ethla.Common.Generating;

public class FeatureClay : Feature
{

	private readonly Func<Block> block;

	private readonly float spread;

	public FeatureClay(float spread, float clusters, Func<Block> block)
	{
		IsSurfacePlaced = false;
		TryTimesPerChunk = clusters;
		this.spread = spread;
		this.block = block;
		Range = new Vector2(Chunk.YOfSea, Chunk.MaxY);
	}

	public override bool IsPlacable(Level level, int x, int y, Seed seed)
	{
		BlockState state = level.GetBlock(x, y);
		return state.Is(Groups.Dirt);
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
				if (d >= spread * 0.8f) c = (spread - d) * 0.1f + 0.5f;
				if ((c == 1 || seed.NextFloat() < c) && IsPlacable(level, x1, y1, seed))
				{
					BlockState state = level.GetBlock(x1, y1);
					level.SetBlock(block().MakeState(state.Meta), x1, y1);
				}
			}
	}

}
