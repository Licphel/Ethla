using Ethla.Api;
using Ethla.World;
using Ethla.World.Generating;
using Ethla.World.Voxel;
using Spectrum.Maths.Random;
using Spectrum.Utils;

namespace Ethla.Common.Generating;

public class GeneratorImpl : Generator
{

	private readonly float[] chanceOfBorder = [1.0f, 0.95f, 0.75f, 0.5f];

	private Noise activeness;
	private Noise continent;
	private Noise hardness;

	public Level Level;
	private Noise noise;
	private Noise rainfall;
	private Noise temperature;

	public GeneratorImpl(Level level)
	{
		Level = level;
	}

	public override void SetSeed(Seed seed)
	{
		//pay attention to this noise, it shouldn't have sth to do with the section coord!
		noise = new NoiseOctave(seed.Copyx(183), 2); //some magic numbers...

		rainfall = new NoisePerlin(seed.Copyx(923));
		temperature = new NoisePerlin(seed.Copyx(1256));
		activeness = new NoiseVoronoi(seed.Copyx(1847));
		hardness = new NoisePerlin(seed.Copyx(1998));
		continent = new NoisePerlin(seed.Copyx(2508));
	}

	public override void Provide(int coord)
	{
		Chunk chunk = Level.GetChunk(coord);
		if (chunk != null)
			return;

		chunk = ProvideEmpty(coord);

		if (Level.ChunkIo.IsChunkArchived(chunk))
		{
			Level.ChunkIo.Read(chunk);
			submitToLevel(chunk, coord);
			return;
		}

		provide(chunk, coord);
	}

	public override Chunk ProvideEmpty(int coord)
	{
		Chunk chunk = new Chunk(Level, coord);
		Level.SetChunk(coord, chunk);
		return chunk;
	}

	public override bool ProvideAsync(int coord)
	{
		Chunk chunk = Level.GetChunk(coord);
		if (chunk != null)
			return true;

		chunk = ProvideEmpty(coord);

		if (Level.ChunkIo.IsChunkArchived(chunk))
		{
			Level.ChunkIo.Read(chunk);
			submitToLevel(chunk, coord);
			return true;
		}

		new Coroutine(() => { provide(chunk, coord); }).Start();
		return false;
	}

	public override void GetLocationData(int x, int y, int surface, GenerateContext ctx)
	{
		float temp = ctx.Temp = temperature.Generate(x / 512f, y / 256f, 1);
		float rain = ctx.Rain = rainfall.Generate(x / 512f, y / 256f, 1);
		float act = ctx.Act = activeness.Generate(x / 128f, y / 128f, 1);
		float dep;
		float cont = ctx.Cont = continent.Generate(x / 512f, 1, 1);

		if (y >= Chunk.YOfSea + Chunk.ScaleOfCrust || y <= Chunk.YOfSea - Chunk.ScaleOfCrust)
			cont = 1;

		if (y >= surface)
			dep = (float)(y - surface) / (Chunk.Height - surface);
		else
			dep = -(float)(surface - y) / surface;

		Biome bm = null;
		float sm = float.PositiveInfinity;

		Biome biome;
		List<Biome> list = ModRegistry.Biomes.IdList;

		for (int i = 0; i < list.Count; i++)
		{
			biome = list[i];

			float s = 0;

			s += incS(biome.Temperature, temp);
			s += incS(biome.Rainfall, rain);
			s += incS(biome.Activeness, act);
			s += incS(biome.Depth, dep) * 16; //Ensure they generate at correct depth.
			s += incS(biome.Continent, cont) * 2;
			s += biome.Rarity;

			if (sm > s)
			{
				sm = s;
				bm = biome;
			}
		}

		ctx.Biome = bm;
	}

	private void submitToLevel(Chunk chunk, int coord)
	{
		Chunk mask = Level.ConsumeChunkMask(coord);
		if (mask != null)
		{
			mask.BlockMap.Cover(chunk.BlockMap);
			mask.LiquidMap.Cover(chunk.LiquidMap);
			mask.WallMap.Cover(chunk.WallMap);
		}

		chunk.OnLoaded();
		Level.SetChunk(coord, chunk);
	}

	private void provide(Chunk chunk, int coord)
	{
		foreach (Decorator decorator in Decorators) decorator.InitSeed(Level, chunk);

		Seed seed = Level.Seed.Copyx(87 * chunk.Coord);
		GenerateContext ctx = new GenerateContext();

		for (int s = 0; s < 16; s++)
		{
			int x = s + coord * 16;

			float hd = 1;
			float sc = Chunk.ScaleOfCrust;
			float surface = noise.Generate(x * hd / 64f, 1, 1) * (3 * hardness.Generate(x / 64f, 1, 1)) * sc + Chunk.YOfSea;
			chunk.Surface[s] = (byte)surface;

			for (int y = Chunk.MaxY; y >= 0; y--)
			{
				GetLocationData(x, y, (int)surface, ctx);
				Biome biome = ctx.Biome;
				chunk.SetBiome(biome, x, y);

				int dirtdep = ctx.Rain switch
				{
					<= 0.25f => 0,
					<= 0.35f => 1,
					<= 0.48f => 2,
					<= 0.64f => 3,
					<= 0.78f => 4,
					_ => 5
				};

				if (y <= surface)
				{
					int dist = (int)surface - y;
					if (dist == 0 && dirtdep > 0)
					{
						chunk.SetBlock(biome.BiomeStates[0], x, y);
						chunk.SetWall(biome.BiomeWalls[0], x, y);
					}
					else if (dist <= dirtdep && (dirtdep == 1 || seed.NextFloat() < (dirtdep + 1 - dist) / 2f))
					{
						chunk.SetBlock(biome.BiomeStates[1], x, y);
						chunk.SetWall(biome.BiomeWalls[1], x, y);
					}
					else if (dist <= (dirtdep + 1) * 2 && seed.NextFloat() < ((dirtdep + 1) * 2 + 1 - dist) / 2f)
					{
						chunk.SetBlock(biome.BiomeStates[2], x, y);
						chunk.SetWall(biome.BiomeWalls[2], x, y);
					}
					else
					{
						if (y <= 3 && seed.NextFloat() <= chanceOfBorder[y])
						{
							chunk.SetBlock(Blocks.Border.MakeState(), x, y);
						}
						else
						{
							chunk.SetBlock(biome.BiomeStates[3], x, y);
						}
						chunk.SetWall(biome.BiomeWalls[3], x, y);
					}
				}

				for (int p = 0; p < DecoratorsFollowing.Count; p++)
					DecoratorsFollowing[p].Decorate(Level, chunk, x, y);
			}
		}

		if (DecoratorsWaiting.Count != 0)
			for (int s = 0; s < 16; s++)
			{
				int x = s + coord * 16;

				for (int y = Chunk.MaxY; y >= 0; y--)
					for (int p = 0; p < DecoratorsWaiting.Count; p++)
						DecoratorsWaiting[p].Decorate(Level, chunk, x, y);
			}

		if (DecoratorsSurface.Count != 0)
			for (int s = 0; s < 16; s++)
			{
				int x = s + coord * 16;
				int y = chunk.GetSurface(s);

				for (int p = 0; p < DecoratorsSurface.Count; p++)
				{
					Decorator dec = DecoratorsSurface[p];
					dec.Decorate(Level, chunk, x, y + dec.SurfaceOffset);
				}
			}

		foreach (PostProcessor processor in Processors) processor.Process(Level, chunk);

		submitToLevel(chunk, coord);
	}

	private static float incS(float m, float v)
	{
		if (m == -1)
			return 1.125f;
		return (float)Math.Pow(Math.Abs(m - v) + 1, 2);
	}

}
