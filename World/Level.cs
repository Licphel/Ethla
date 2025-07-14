using System.Collections.Concurrent;
using Ethla.Common.Mob;
using Ethla.Util;
using Ethla.World.Generating;
using Ethla.World.Lighting;
using Ethla.World.Mob;
using Ethla.World.Physics;
using Ethla.World.Voxel;
using Spectrum.Codec;
using Spectrum.Core;
using Spectrum.IO.Bin;
using Spectrum.Maths;
using Spectrum.Maths.Random;
using Spectrum.Utils;

namespace Ethla.World;

public class Level : LevelBasic
{

	public readonly ConcurrentDictionary<int, Chunk> ChunkMap = new ConcurrentDictionary<int, Chunk>();
	public readonly ConcurrentDictionary<int, Chunk> ChunkMaskMap = new ConcurrentDictionary<int, Chunk>();

	public readonly HashSet<Chunk> ChunkSetSafe = new HashSet<Chunk>();
	public Queue<Action> ChunkActionsQueue = new Queue<Action>();
	public ChunkManager ChunkIo;

	public Climate Climate = new Climate();
	public Dictionary<Uuid, Entity> EntitiesById = new Dictionary<Uuid, Entity>();
	public Generator Generator;

	public ImVector2 Gravity = new ImVector2(0, -9.8f);
	public Seed Seed = new SeedXoroshiro();
	public LevelType Type;

	public Level(LevelType type)
	{
		Type = type;
		LightEngine = new LightEngineImpl(this);
		LowpEntities = new LowpEntityList(this);
		Generator = type.ProviderGenerator(this);
		Generator.SetSeed(Seed);
		ChunkIo = new ChunkManager(type);
	}

	public override LightEngine LightEngine { get; }
	public override LowpEntityList LowpEntities { get; }

	public override IEnumerable<Chunk> ActiveChunks => ChunkSetSafe;
	public virtual long TicksPerDay => 60 * 60 * 24;

	public override Chunk GetChunk(int coord)
	{
		return ChunkMap.GetValueOrDefault(coord, null);
	}

	public override void SetChunk(int coord, Chunk chunk)
	{
		ChunkMap[coord] = chunk;

		ChunkActionsQueue.Enqueue(() => { ChunkSetSafe.Add(chunk); });
	}

	public override void RemoveChunk(int coord)
	{
		Chunk chk = GetChunk(coord);
		ChunkMap.Remove(coord, out Chunk _);

		if (chk != null)
			ChunkActionsQueue.Enqueue(() =>
			{
				ChunkSetSafe.Remove(chk);
				chk.OnUnloaded();
			});
	}

	public Chunk ConsumeChunkMask(int coord)
	{
		Chunk msk = ChunkMaskMap.GetValueOrDefault(coord, null);
		if (msk != null)
			ChunkMaskMap.Remove(coord, out Chunk _);
		return msk;
	}

	public override BlockState GetBlock(int x, int y)
	{
		int coord = Posing.ToCoord(x);
		Chunk chunk = GetChunk(coord);
		if (chunk == null)
		{
			Chunk mask = ChunkMaskMap.GetValueOrDefault(coord, null);
			if (mask == null)
				return BlockState.Empty;
			return mask.GetBlock(x, y);
		}

		return chunk.GetBlock(x, y);
	}

	public override void SetBlock(BlockState state, int x, int y, long flag = SetBlockFlag.NoFlag)
	{
		int cx = Posing.ToCoord(x);
		Chunk chunk = GetChunk(cx);
		if (chunk == null)
		{
			Chunk mask;

			if (ChunkMaskMap.TryGetValue(cx, out Chunk mask0))
				mask = mask0;
			else
				ChunkMaskMap[cx] = mask = new Chunk(this, cx);

			mask.SetBlock(state, x, y, flag);
			return;
		}

		chunk.SetBlock(state, x, y, flag);
	}

	public override Wall GetWall(int x, int y)
	{
		int coord = Posing.ToCoord(x);
		Chunk chunk = GetChunk(coord);
		if (chunk == null)
		{
			Chunk mask = ChunkMaskMap.GetValueOrDefault(coord, null);
			if (mask == null)
				return Wall.Empty;
			return mask.GetWall(x, y);
		}

		return chunk.GetWall(x, y);
	}

	public override void SetWall(Wall wall, int x, int y)
	{
		int cx = Posing.ToCoord(x);
		Chunk chunk = GetChunk(cx);
		if (chunk == null)
		{
			Chunk mask;

			if (ChunkMaskMap.TryGetValue(cx, out Chunk mask0))
				mask = mask0;
			else
				ChunkMaskMap[cx] = mask = new Chunk(this, cx);

			mask.SetWall(wall, x, y);
			return;
		}

		chunk.SetWall(wall, x, y);
	}

	public override LiquidStack GetLiquid(int x, int y)
	{
		int coord = Posing.ToCoord(x);
		Chunk chunk = GetChunk(coord);
		if (chunk == null)
		{
			Chunk mask = ChunkMaskMap.GetValueOrDefault(coord, null);
			if (mask == null)
				return new LiquidStack();
			return mask.GetLiquid(x, y);
		}

		return chunk.GetLiquid(x, y);
	}

	public override void SetLiquid(LiquidStack stack, int x, int y)
	{
		int cx = Posing.ToCoord(x);
		Chunk chunk = GetChunk(cx);
		if (chunk == null)
		{
			Chunk mask;

			if (ChunkMaskMap.TryGetValue(cx, out Chunk mask0))
				mask = mask0;
			else
				ChunkMaskMap[cx] = mask = new Chunk(this, cx);

			mask.SetLiquid(stack, x, y);
			return;
		}

		chunk.SetLiquid(stack, x, y);
	}

	public override Biome GetBiome(int x, int y)
	{
		int coord = Posing.ToCoord(x);
		Chunk chunk = GetChunk(coord);
		if (chunk == null)
		{
			Chunk mask = ChunkMaskMap.GetValueOrDefault(coord, null);
			if (mask == null)
				return Biome.Unknown;
			return mask.GetBiome(x, y);
		}

		return chunk.GetBiome(x, y);
	}

	public override void SetBiome(Biome biome, int x, int y)
	{
		int cx = Posing.ToCoord(x);
		Chunk chunk = GetChunk(cx);
		if (chunk == null)
		{
			Chunk mask;

			if (ChunkMaskMap.TryGetValue(cx, out Chunk mask0))
				mask = mask0;
			else
				ChunkMaskMap[cx] = mask = new Chunk(this, cx);

			mask.SetBiome(biome, x, y);
			return;
		}

		chunk.SetBiome(biome, x, y);
	}

	public BlockState BreakBlock(Pos pos, DamageSource src = null, bool drop = true)
	{
		BlockState state = GetBlock(pos);
		if (state.IsEmpty)
			return BlockState.Empty;
		if (drop)
				EntityItem.PopDrop(this, state.GetDrop(this, new BlockPos(pos)), pos);
		PlayDestructParticles(state, pos);
		state.GetSound("destruct").PlaySound(pos);
		state.OnDestroyed(src, new BlockPos(pos));
		SetBlock(BlockState.Empty, pos);
		return state;
	}

	public Wall BreakWall(Pos pos, DamageSource src = null, bool drop = true)
	{
		Wall wall = GetWall(pos);
		if (wall == Wall.Empty)
			return Wall.Empty;
		if (drop)
				EntityItem.PopDrop(this, wall.GetDrop(this, new BlockPos(pos)), pos);
		PlayDestructParticles(wall, pos);
		wall.GetSound("destruct").PlaySound(pos);
		wall.OnDestroyed(src, new BlockPos(pos));
		SetWall(Wall.Empty, pos);
		return wall;
	}

	public int GetSurface(int x)
	{
		Chunk chunk = GetChunkByBlock(x);
		if (chunk == null)
			return 0;
		return chunk.GetSurface(x);
	}

	public int GetSurface(Pos pos)
	{
		return GetSurface(pos.BlockX);
	}

	public override void SpawnEntity(Entity entity, Pos pos)
	{
		if (!IsAccessible(pos)) return;

		ChunkUnit unit = GetUnit(pos);

		if (unit == null) return;

		entity.Locate(pos, false);

		entity.Level = this;
		entity.AddedToChunk = true;
		entity.ChunkUnit = unit;

		unit.AddEntity(entity);
		entity.OnSpawned();
		EntitiesById[entity.UniqueId] = entity;

		entity.RegrabLight(true);
	}

	public void RemoveEntity(Entity entity)
	{
		EntitiesById.Remove(entity.UniqueId);
		entity.OnDespawned();
	}

	public void RemoveEntityInstantly(Entity entity)
	{
		entity.ChunkUnit.RemoveEntity(entity, true);
	}

	//LOGIC

	public void Tick()
	{
		LowpEntities.Tick();
		Climate.Tick();

		foreach (Chunk chunk in ActiveChunks) chunk.Tick();

		while (ChunkActionsQueue.Count > 0)
			ChunkActionsQueue.Dequeue()?.Invoke();

		if (TimeSchedule.PeriodicTask(1))
		{
			HashSet<Chunk> set1 = new HashSet<Chunk>(ChunkMap.Values);
			HashSet<Chunk> set2 = ChunkSetSafe;

			if (!set1.SetEquals(set2)) Logger.Fatal("Some chunks go wrong - Maybe they are missing. Crashed!");
		}

		Ticks++;
		Seconds += Time.Delta;
		ChunkIo.CheckOldBuffers(this);

		if (TimeSchedule.PeriodicTask(20))
			Save();
	}

	public void TryLoad()
	{
		if (ChunkIo.LevelSave.Exists)
		{
			BinaryCompound overall = BinaryRw.Read(ChunkIo.LevelSave);

			Seed.Read(overall.GetCompoundSafely("seed"));

			BinaryList list = overall.GetListSafely("chunk_masks");

			foreach (BinaryCompound compound in list)
			{
				int coord = compound.Get<int>("coord");
				Chunk mask = new Chunk(this, coord);
				mask.Read(compound.GetCompoundSafely("data"));
				ChunkMaskMap[coord] = mask;
			}

			Ticks = overall.GetOrDefault("time", 0);
			Seconds = overall.GetOrDefault("time_seconds", 0);
		}

		Generator.SetSeed(Seed);
	}

	public void Save()
	{
		Logger.Info("Level's saving...", true);

		foreach (Chunk chunk in ActiveChunks)
			ChunkIo.WriteToBuffer(chunk);
		ChunkIo.WriteToDisk();

		BinaryCompound overall = BinaryCompound.New();

		Seed.Write(overall.NewScope("seed"));

		//chunk masks
		BinaryList list = BinaryList.New();

		foreach (KeyValuePair<int, Chunk> kv in ChunkMaskMap)
		{
			BinaryCompound compound = BinaryCompound.New();
			compound.Set("coord", kv.Key);
			kv.Value.Write(compound.NewScope("data"));
			list.Insert(compound);
		}

		overall.Set("chunk_masks", list);

		overall.Set("time", Ticks);
		overall.Set("time_seconds", Seconds);

		BinaryRw.Write(overall, ChunkIo.LevelSave);
		Main.SavePlayer();

		Logger.Fix("Done!");
	}

}
