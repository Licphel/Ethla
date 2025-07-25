﻿using Ethla.World;
using Ethla.World.Voxel;

namespace Ethla.Common.Voxel;

public class BlockWallLandAttach : Block
{

	public override bool CheckSustainability(BlockState state, Level level, BlockPos pos)
	{
		if (level.GetWall(pos).GetShape() != BlockShape.Solid && level.GetBlock(Direction.Down.Step(pos)).GetShape() != BlockShape.Solid)
			return false;
		return true;
	}

}
