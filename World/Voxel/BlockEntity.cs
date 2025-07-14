using Ethla.World.Iteming;
using Spectrum.IO.Bin;

namespace Ethla.World.Voxel;

public class BlockEntity
{

	public bool Dirty;

	public Level Level;
	public BlockState PeerBlock;
	public BlockPos Pos;

	public BlockEntity(BlockState state, Level level, BlockPos pos)
	{
		Level = level;
		Pos = pos;
		PeerBlock = state;
	}

	public virtual void OnSpawned()
	{
	}

	public virtual void OnDespawned()
	{
	}

	public virtual void OnBeenReplaceByNewEntity(BlockEntity newEntity)
	{
	}

	public virtual void OverrideBlockDrops(List<ItemStack> stacks)
	{
	}

	public virtual void Write(BinaryCompound compound)
	{
	}

	public virtual void Read(BinaryCompound compound)
	{
	}

}
