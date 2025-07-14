using Ethla.World;
using Ethla.World.Generating;
using Ethla.World.Voxel;
using Spectrum.Maths.Random;

namespace Ethla.Common.Voxel;

public class BlockSeedling : BlockPlant
{

	private readonly Func<Feature> featureFac;

	public BlockSeedling(Func<Feature> feature)
	{
		featureFac = feature;
	}

	public override void OnRandomTick(BlockState state, Level level, BlockPos pos)
	{
		base.OnRandomTick(state, level, pos);

		Seed seed = Seed.Global;
		Feature feature = featureFac();

		if (feature.IsPlacable(level, pos.BlockX, pos.BlockY - 1, seed))
			feature.Place(level, pos.BlockX, pos.BlockY - 1, seed);
	}

}
