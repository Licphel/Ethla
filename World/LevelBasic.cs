using Ethla.Common.Particles;
using Ethla.World.Generating;
using Ethla.World.Lighting;
using Ethla.World.Mob;
using Ethla.World.Physics;
using Ethla.World.Voxel;
using Spectrum.Maths;
using Spectrum.Maths.Shapes;

namespace Ethla.World;

public abstract class LevelBasic
{

	private static readonly List<Entity> LevelETemp = new List<Entity>();
	public float Seconds;
	public long Ticks;

	public abstract LightEngine LightEngine { get; }
	public abstract LowpEntityList LowpEntities { get; }
	public abstract IEnumerable<Chunk> ActiveChunks { get; }

	public Chunk GetChunk(Pos pos) => GetChunk(pos.UnitX);
	public Chunk GetChunkByBlock(int blockx) => GetChunk(Posing.ToCoord(blockx));
	public abstract Chunk GetChunk(int coord);

	public ChunkUnit GetUnit(Pos pos)
	{
		Chunk chunk = GetChunk(pos);
		return chunk == null ? null : chunk.GetUnitSafely(pos.UnitY);
	}

	public void SetChunk(Pos pos, Chunk chunk) => SetChunk(pos.UnitX, chunk);
	public abstract void SetChunk(int coord, Chunk chunk);
	public abstract void RemoveChunk(int coord);

	public void GetNearbyBlocks(List<BlockPos> list, Quad dim)
	{
		list.Clear();
		Chunk chunk = null;

		for (float i = dim.X - dim.W / 2f - 1; i <= dim.Xprom + dim.W / 2f + 1; i++)
		{
			int x = Mathf.FastFloor(i);
			int coord = Posing.ToCoord(x);

			if (chunk == null || chunk.Coord != coord) chunk = GetChunk(coord);
			if (chunk == null) continue;

			for (float j = dim.Y - dim.H / 2f - 1; j <= dim.Yprom + dim.H / 2f + 1; j++)
			{
				int y = Mathf.FastFloor(j);

				BlockState state = chunk.GetBlock(x, y);
				BlockPos pos = new BlockPos(x, y);
				if (!state.IsEmpty && !list.Contains(pos))
					list.Add(pos);
			}
		}
	}

	public void GetNearbyLiquids(List<BlockPos> list, Quad dim)
	{
		list.Clear();
		Chunk chunk = null;

		for (float i = dim.X - dim.W / 2f - 1; i <= dim.Xprom + dim.W / 2f + 1; i++)
		{
			int x = Mathf.FastFloor(i);
			int coord = Posing.ToCoord(x);

			if (chunk == null || chunk.Coord != coord) chunk = GetChunk(coord);
			if (chunk == null) continue;

			for (float j = dim.Y - dim.H / 2f - 1; j <= dim.Yprom + dim.H / 2f + 1; j++)
			{
				int y = Mathf.FastFloor(j);

				LiquidStack stack = chunk.GetLiquid(x, y);
				BlockPos pos = new BlockPos(x, y);
				if (stack.Amount > 0 && !list.Contains(pos))
					list.Add(pos);
			}
		}
	}

	public BlockState GetBlock(Pos pos) => GetBlock(pos.BlockX, pos.BlockY);
	public abstract BlockState GetBlock(int x, int y);
	public void SetBlock(BlockState state, Pos grid, long flag = SetBlockFlag.NoFlag) => SetBlock(state, grid.BlockX, grid.BlockY, flag);
	public abstract void SetBlock(BlockState state, int x, int y, long flag = SetBlockFlag.NoFlag);
	public Wall GetWall(Pos pos) => GetWall(pos.BlockX, pos.BlockY);
	public abstract Wall GetWall(int x, int y);
	public void SetWall(Wall wall, Pos grid) => SetWall(wall, grid.BlockX, grid.BlockY);
	public abstract void SetWall(Wall wall, int x, int y);
	public virtual LiquidStack GetLiquid(Pos pos) => GetLiquid(pos.BlockX, pos.BlockY);
	public abstract LiquidStack GetLiquid(int x, int y);
	public abstract void SetLiquid(LiquidStack stack, int x, int y);
	public virtual void SetLiquid(LiquidStack stack, Pos pos) => SetLiquid(stack, pos.BlockX, pos.BlockY);
	public abstract Biome GetBiome(int x, int y);
	public abstract void SetBiome(Biome biome, int x, int y);

	public void PlayDestructParticles(object o, Pos pos, int count = 16)
	{
		for (int i = 0; i < count; i++)
		{
			ParticleBlockDust p = new ParticleBlockDust(o);
			LowpEntities.AddSpreading(p, pos, 0.5f);
		}
	}

	public BlockEntity GetBlockEntity(Pos pos)
	{
		Chunk chunk = GetChunk(pos);
		return chunk == null ? null : chunk.GetBlockEntity(pos);
	}

	public BlockEntity GetBlockEntity(int x, int y) => GetBlockEntity(new BlockPos(x, y));

	public void SetBlockEntity(BlockEntity e, Pos pos)
	{
		Chunk chunk = GetChunk(pos);
		if (chunk == null) return;
		chunk.SetBlockEntity(e, pos);
	}

	public void SetBlockEntity(BlockEntity e, int x, int y) => SetBlockEntity(e, new BlockPos(x, y));

	public void SpawnEntity(Entity entity, float x, float y) => SpawnEntity(entity, new PrecisePos(x, y));
	public void SpawnEntity(Entity entity) => SpawnEntity(entity, entity.Pos);

	public abstract void SpawnEntity(Entity entity, Pos pos);

	public List<Entity> GetNearbyEntities(Quad aabb, Type predicate = null, int offset = 2)
	{
		return GetNearbyEntities(LevelETemp, aabb, predicate, offset);
	}

	public List<Entity> GetNearbyEntities(List<Entity> buffer, Quad aabb, Type predicate = null, int offset = 2)
	{
		if (predicate == null)
			predicate = typeof(Entity);

		Chunk chunk = null;

		buffer.Clear();
		int i1 = Mathf.FastFloor((aabb.X - offset) / 16f);
		int i2 = Mathf.FastFloor((aabb.Xprom + offset) / 16f);
		int j1 = Mathf.FastFloor((aabb.Y - offset) / 16f);
		int j2 = Mathf.FastFloor((aabb.Yprom + offset) / 16f);

		for (int i = i1; i <= i2; i++)
		{
			if (chunk == null || chunk.Coord != i) chunk = GetChunk(i);

			if (chunk == null) continue;

			for (int j = j1; j <= j2; j++)
			{
				List<object> entityList = chunk.GetUnitSafely(j).EntitySortMap.Get(predicate);

				for (int k = 0; k < entityList.Count; k++)
				{
					Entity e = (Entity)entityList[k];

					if (e == null || e.ShouldRemove) continue;
					if (e.Bound.Interacts(aabb) && !buffer.Contains(e)) buffer.Add(e);
				}
			}
		}

		return buffer;
	}

	//Utilities

	public bool IsAccessible(Pos pos)
	{
		if (pos.Y < 0 || pos.Y >= Chunk.Height) return false;
		Chunk chk = GetChunk(pos);
		return chk != null; //do not check whether it is loaded. entity serial will collapse.
	}

	public static bool IsYPosAccessible(Pos pos)
	{
		return IsYPosAccessible(pos.Y);
	}

	public static bool IsYPosAccessible(float y)
	{
		if (y < 0 || y >= Chunk.Height) return false;
		return true;
	}

}
