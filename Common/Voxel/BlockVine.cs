using Ethla.World;
using Ethla.World.Voxel;

namespace Ethla.Common.Voxel;

public class BlockVine : Block
{

	public override bool CheckSustainability(BlockState state, Level level, BlockPos pos)
	{
		BlockState state1 = level.GetBlock(Direction.Up.Step(pos));
		if (!state1.GetShape().IsFull && !state1.Is(this))
			return false;
		return true;
	}

}
