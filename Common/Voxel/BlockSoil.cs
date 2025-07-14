using Ethla.World;
using Ethla.World.Voxel;
using Spectrum.Maths.Random;

namespace Ethla.Common.Voxel;

public class BlockDirt : Block
{

	public override void OnRandomTick(BlockState state, Level level, BlockPos pos)
	{
		if (state.Meta == 0)
			return;

		bool upq = level.GetBlock(Direction.Up.Step(pos)).GetShape() == BlockShape.Solid;
		if (upq)
		{
			level.SetBlock(MakeState(), pos);
			return;
		}

		for (int i = 0; i < 3; i++)
		{
			BlockPos pos2 = pos.Offset(Seed.Global.NextInt(-2, 2), Seed.Global.NextInt(-2, 2));

			BlockState t2 = level.GetBlock(pos2);
			upq = level.GetBlock(Direction.Up.Step(pos2)).GetShape() == BlockShape.Solid;

			if (t2.Is(state) && t2.Meta == 0 && !upq) level.SetBlock(MakeState(1), pos2);
		}
	}

}
