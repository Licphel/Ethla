using Ethla.World;
using Ethla.World.Voxel;

namespace Ethla.Common.Voxel;

public class BlockPlant : Block
{

	public override bool CheckSustainability(BlockState state, Level level, BlockPos pos)
	{
		if (!level.GetBlock(Direction.Down.Step(pos)).Is(Groups.Dirt))
			return false;
		return true;
	}

}
