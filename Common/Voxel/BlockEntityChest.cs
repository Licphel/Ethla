using Ethla.World;
using Ethla.World.Iteming;
using Ethla.World.Voxel;
using Spectrum.IO.Bin;

namespace Ethla.Common.Voxel;

public class BlockEntityChest : BlockEntity
{

	public Inventory Inv = new Inventory(27);

	public BlockEntityChest(BlockState state, Level level, BlockPos pos) : base(state, level, pos)
	{
	}

	public override void OverrideBlockDrops(List<ItemStack> stacks)
	{
		base.OverrideBlockDrops(stacks);

		foreach (ItemStack stk in Inv)
			if (!stk.IsEmpty)
				stacks.Add(stk);
	}

	public override void Read(BinaryCompound compound)
	{
		base.Read(compound);
		Inv = Inventory.Deserialize(compound.GetCompoundSafely("cinv"));
	}

	public override void Write(BinaryCompound compound)
	{
		base.Write(compound);
		Inventory.Serialize(Inv, compound.NewScope("cinv"));
	}

}
