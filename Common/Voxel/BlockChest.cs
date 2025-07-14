using Ethla.Common.Iteming;
using Ethla.Common.Mob;
using Ethla.World;
using Ethla.World.Iteming;
using Ethla.World.Voxel;

namespace Ethla.Common.Voxel;

public class BlockChest : Block
{

	public override BlockEntity CreateEntityBehavior(BlockState state, Level level, BlockPos pos)
	{
		return new BlockEntityChest(state, level, pos);
	}

	public override SlotLayout CreateCaridge(BlockPos pos, EntityPlayer player)
	{
		return new SlotLayoutChest(pos, player);
	}

	public override bool HasCaridge(BlockPos pos, EntityPlayer player)
	{
		return true;
	}

	public override bool HasEntityBehavior(BlockState state, Level level, BlockPos pos)
	{
		return true;
	}

}
