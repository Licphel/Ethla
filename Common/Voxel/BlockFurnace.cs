using Ethla.Common.Iteming;
using Ethla.Common.Mob;
using Ethla.World;
using Ethla.World.Iteming;
using Ethla.World.Voxel;

namespace Ethla.Common.Voxel;

public class BlockFurnace : Block
{

	public override bool HasEntityBehavior(BlockState state, Level level, BlockPos pos)
	{
		return true;
	}

	public override BlockEntity CreateEntityBehavior(BlockState state, Level level, BlockPos pos)
	{
		return new BlockEntityFurnace(state, level, pos);
	}

	public override SlotLayout CreateCaridge(BlockPos pos, EntityPlayer player)
	{
		return new SlotLayoutFurnace(pos, player);
	}

	public override bool HasCaridge(BlockPos pos, EntityPlayer player)
	{
		return true;
	}

}
