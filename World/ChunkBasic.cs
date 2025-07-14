using Ethla.World.Generating;
using Ethla.World.Voxel;

namespace Ethla.World;

public abstract class ChunkBasic
{

	public abstract ChunkUnit[] Units { get; }

	public virtual ChunkUnit GetUnitSafely(int ycoord)
	{
		ycoord = Math.Clamp(ycoord, 0, Units.Length - 1);
		return Units[ycoord];
	}

	public virtual BlockState GetBlock(Pos pos) => GetBlock(pos.BlockX, pos.BlockY);
	public abstract BlockState GetBlock(int x, int y);
	public abstract void SetBlock(BlockState state, int x, int y, long flag = SetBlockFlag.NoFlag);
	public virtual void SetBlock(BlockState state, Pos pos, long flag = SetBlockFlag.NoFlag) => SetBlock(state, pos.BlockX, pos.BlockY, flag);

	public virtual Wall GetWall(Pos pos) => GetWall(pos.BlockX, pos.BlockY);
	public abstract Wall GetWall(int x, int y);
	public abstract void SetWall(Wall wall, int x, int y);
	public virtual void SetWall(Wall wall, Pos pos) => SetWall(wall, pos.BlockX, pos.BlockY);

	public virtual LiquidStack GetLiquid(Pos pos) => GetLiquid(pos.BlockX, pos.BlockY);
	public abstract LiquidStack GetLiquid(int x, int y);
	public abstract void SetLiquid(LiquidStack stack, int x, int y);
	public virtual void SetLiquid(LiquidStack stack, Pos pos) => SetLiquid(stack, pos.BlockX, pos.BlockY);

	public abstract Biome GetBiome(int x, int y);
	public abstract void SetBiome(Biome biome, int x, int y);

	public abstract BlockEntity GetBlockEntity(Pos pos);
	public virtual BlockEntity GetBlockEntity(int x, int y) => GetBlockEntity(new BlockPos(x, y));
	public virtual void SetBlockEntity(BlockEntity entity, int x, int y) => SetBlockEntity(entity, new BlockPos(x, y));
	public abstract void SetBlockEntity(BlockEntity entity, Pos pos);

}
