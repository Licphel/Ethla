using Ethla.World.Generating;
using Ethla.World.Lighting;
using Ethla.World.Mob.Ai;
using Ethla.World.Voxel;
using Spectrum.Core;
using Spectrum.IO.Bin;
using Spectrum.Maths.Random;

namespace Ethla.World;

public sealed class Chunk : ChunkBasic
{

	public const int Width = 16;
	public const int Height = 256;
	public const int Depth = 2;
	public const int Area = Width * Height;
	public const int Volume = Depth * Area;
	public const int MaxY = 255;

	public const int ScaleOfCrust = 24;
	public const int YOfSea = 102;
	public const int YOfMostCave = 48;
	public const int YOfSurface = YOfSea + ScaleOfCrust;
	public const int YOfSpace = 164;

	public static int RefreshTimeNormal = 200;
	public BiomeMap BiomeMap = new BiomeMap();

	public Dictionary<BlockPos, BlockEntity> BlockEntities = new Dictionary<BlockPos, BlockEntity>();
	public BlockMap BlockMap = new BlockMap();
	public WallMap WallMap = new WallMap();
	public int Coord;
	public bool Dirty;
	public Level Level;

	//[0] to storage block light and coords out for z = 0,
	//[1] to storage sky light and coords out for z = 1.
	private LightWare?[,] lightStorages = new LightWare?[Width, Height];
	private LightWare?[,] lightStoragesBuffer = new LightWare?[Width, Height];
	public LiquidMap LiquidMap = new LiquidMap();
	public int RefreshTime;
	public byte[] Surface = new byte[16];
	public List<Tickable> Tickables = new List<Tickable>();
	public int TicksSinceSaving;

	public Chunk(Level level, int coord)
	{
		Level = level;
		Coord = coord;

		for (int i = 0; i < Units.Length; i++) Units[i] = new ChunkUnit(this, new UnitPos(coord, i));
	}

	public override ChunkUnit[] Units { get; } = new ChunkUnit[Height / Width];

	public bool IsLoadedUp { get; set; }

	public void OnLoaded()
	{
		IsLoadedUp = true;
		Dirty = true;
	}

	public void OnUnloaded()
	{
		IsLoadedUp = false;
	}

	public Seed GetUniqSeed()
	{
		return Level.Seed.Copyx(Coord * 3);
	}

	public override BlockState GetBlock(int x, int y)
	{
		if (!LevelBasic.IsYPosAccessible(y))
			return BlockState.Empty;
		return BlockMap.Get(x, y);
	}

	public override void SetBlock(BlockState state, int x, int y, long flag = SetBlockFlag.NoFlag)
	{
		if (!LevelBasic.IsYPosAccessible(y))
			return;

		BlockPos pos = new BlockPos(x, y);
		BlockMap.Set(x, y, state);

		if (state.GetShape() == BlockShape.Solid)
			LiquidMap.Set(x, y, new LiquidStack());

		BlockEntity entity = state.CreateEntityBehavior(Level, pos);
		SetBlockEntity(entity, pos);
		Dirty = true;

		if (IsLoadedUp)
		{
			NotifyChange(pos, flag);
		}
	}

	public override LiquidStack GetLiquid(int x, int y)
	{
		if (!LevelBasic.IsYPosAccessible(y))
			return new LiquidStack();
		return LiquidMap.Get(x, y);
	}

	public override void SetLiquid(LiquidStack stack, int x, int y)
	{
		if (!LevelBasic.IsYPosAccessible(y))
			return;
		LiquidMap.Set(x, y, stack);
	}

	public override Biome GetBiome(int x, int y)
	{
		if (!LevelBasic.IsYPosAccessible(y))
			return Biome.Unknown;
		return BiomeMap.Get(x, y);
	}

	public override void SetBiome(Biome biome, int x, int y)
	{
		if (!LevelBasic.IsYPosAccessible(y))
			return;
		BiomeMap.Set(x, y, biome);
	}

	public override Wall GetWall(int x, int y)
	{
		if (!LevelBasic.IsYPosAccessible(y))
			return Wall.Empty;
		return WallMap.Get(x, y);
	}

	public override void SetWall(Wall wall, int x, int y)
	{
		if (!LevelBasic.IsYPosAccessible(y))
			return;

		BlockPos pos = new BlockPos(x, y);
		WallMap.Set(x, y, wall);

		Dirty = true;
	}

	public void NotifyChange(BlockPos pos, long flag)
	{
		if (SetBlockFlag.Has(flag, SetBlockFlag.Isolated))
			return;

		foreach (Direction d in Direction.Values)
		{
			BlockPos pos1 = d.Step(pos);
			BlockState state1 = Level.GetBlock(pos1);
			if (!state1.CheckSustainability(Level, pos1))
				Level.BreakBlock(pos1);
		}
	}

	public override BlockEntity GetBlockEntity(Pos pos)
	{
		return BlockEntities.GetValueOrDefault(pos is BlockPos tp ? tp : new BlockPos(pos), null);
	}

	public override void SetBlockEntity(BlockEntity entity, Pos pos)
	{
		BlockPos tp = new BlockPos(pos);
		BlockEntity t0 = GetBlockEntity(tp);

		t0?.OnDespawned();
		BlockEntities.Remove(tp);

		if (t0 != null && t0 is Tickable tk) Tickables.Remove(tk);

		if (entity != null)
		{
			BlockEntities[tp] = entity;
			if (entity is Tickable tk1)
				Tickables.Add(tk1);
			entity.OnSpawned();
			t0?.OnBeenReplaceByNewEntity(entity);
		}

		Dirty = true;
	}

	public int GetSurface(int x)
	{
		return Surface[x & 15];
	}

	public int GetSurface(Pos pos)
	{
		return Surface[pos.BlockX & 15];
	}

	//LOGIC

	public void Tick()
	{
		if (!IsLoadedUp) return;

		foreach (ChunkUnit unit in Units)
			unit.Tick();

		foreach (Tickable tk in Tickables)
			tk.Tick();

		for (int i = 0; i < 5; i++)
		{
			BlockPos pos = new BlockPos(Coord * 16 + Seed.Global.NextInt(16), Seed.Global.NextInt(Height));
			GetBlock(pos).OnRandomTick(Level, pos);
		}

		TicksSinceSaving++;
		RefreshTime--;

		if (RefreshTime <= 0)
		{
			Level.ChunkIo.WriteToBuffer(this, true);
			Level.RemoveChunk(Coord);
		}

		if (TimeSchedule.PeriodicTask(1 / 10f))
			Spawner.TryGenerate(this);

		if (TimeSchedule.PeriodicTask(1 / 20f))
			Liquid.TickChunkLiquid(this);
	}

	//Codec

	public void Write(BinaryCompound compound, bool removal = false)
	{
		compound.Set("surfaces", Surface);
		compound.Set("block_bytes", BlockMap.Bytes);
		compound.Set("wall_bytes", WallMap.Bytes);
		compound.Set("liquid_bytes", LiquidMap.Bytes);
		compound.Set("biome_bytes", BiomeMap.Bytes);

		foreach (ChunkUnit unit in Units)
			unit.Write(compound.NewScope("unit_" + unit.Pos.UnitY), removal);

		BinaryList telist = BinaryList.New();

		foreach (BlockEntity te in BlockEntities.Values)
		{
			BinaryCompound c1 = BinaryCompound.New();
			te.Write(c1);
			c1.Set("x", te.Pos.BlockX);
			c1.Set("y", te.Pos.BlockY);
			telist.Insert(c1);
		}

		compound.Set("block_entities", telist);

		compound.Set("chunk_version", Bootstrap.Version.Iteration);
	}

	public void Read(BinaryCompound compound)
	{
		Surface = compound.GetOrDefault("surfaces", new byte[Width]);
		BlockMap.Bytes = compound.GetOrDefault("block_bytes", BlockMap.Bytes);
		WallMap.Bytes = compound.GetOrDefault("wall_bytes", WallMap.Bytes);
		LiquidMap.Bytes = compound.GetOrDefault("liquid_bytes", LiquidMap.Bytes);
		BiomeMap.Bytes = compound.GetOrDefault("biome_bytes", BiomeMap.Bytes);

		foreach (ChunkUnit unit in Units)
			unit.Read(compound.GetCompoundSafely("unit_" + unit.Pos.UnitY));

		BinaryList telist = compound.GetListSafely("block_entities");

		foreach (BinaryCompound c1 in telist)
		{
			int x = c1.Get<int>("x");
			int y = c1.Get<int>("y");
			BlockEntity te = GetBlock(x, y).CreateEntityBehavior(Level, new BlockPos(x, y));
			te.Read(c1);
			SetBlockEntity(te, te.Pos);
		}
	}

	public LightWare GetSurLightware(int x, int y)
	{
		if (y < 0 || y > MaxY)
			return LightWare.Empty;
		LightWare coords = lightStorages[x & 15, y];
		if (coords == null)
		{
			coords = new LightWare(this);
			lightStorages[x & 15, y] = coords;
		}

		return coords;
	}

	public LightWare GetSubLightware(int x, int y)
	{
		if (y < 0 || y > MaxY)
			return LightWare.Empty;
		LightWare coords = lightStoragesBuffer[x & 15, y];
		if (coords == null)
		{
			coords = new LightWare(this);
			lightStoragesBuffer[x & 15, y] = coords;
		}

		return coords;
	}

	public void SwapLightwareBuffer()
	{
		(lightStorages, lightStoragesBuffer) = (lightStoragesBuffer, lightStorages);
	}

}
