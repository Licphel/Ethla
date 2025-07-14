using System;
using Ethla.World;
using Ethla.World.Iteming;
using Ethla.World.Physics;
using Ethla.World.Voxel;

namespace Ethla.Common.Voxel;

public class BlockCactus : Block
{

	public override BlockState GetStateForPlacing(Level level, Entity entity, Pos pos, ItemStack placerItem)
	{
        BlockState state = level.GetBlock(Direction.Up.Step(pos));

        if (state.Is(this))
            return MakeState(0);
        else
            return MakeState(1);
	}

    public override bool CheckSustainability(BlockState state, Level level, BlockPos pos)
    {
        BlockState state1 = level.GetBlock(Direction.Down.Step(pos));
        BlockState state2 = level.GetBlock(Direction.Up.Step(pos));

        if (state2.Is(this))
            level.SetBlock(MakeState(0), pos, SetBlockFlag.Isolated);
        else if (state.Meta == 0)
            level.SetBlock(MakeState(1), pos, SetBlockFlag.Isolated);

        if (!state1.Is(Groups.Sand) && !state1.Is(this))
            return false;
        return true;
    }

}
