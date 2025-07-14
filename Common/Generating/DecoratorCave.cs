using Ethla.World;
using Ethla.World.Generating;
using Ethla.World.Voxel;
using Spectrum.Maths.Random;

namespace Ethla.Common.Generating;

public class DecoratorCave : Decorator
{

	private bool init;
	private Noise noise1;
	private Noise noise2;
	private Noise noise3;
	private Noise noise4;

	public override DecoratorType Type => DecoratorType.Following;

	public override void InitSeed(Level level, Chunk chunk)
	{
		base.InitSeed(level, chunk);

		if (init) return;

		init = true;

		//use level seed. make caves connected.
		noise1 = new NoisePerlin(level.Seed.Copyx(3));
		noise2 = new NoisePerlin(level.Seed.Copyx(6));
		noise3 = new NoisePerlin(level.Seed.Copyx(9));
		noise4 = new NoisePerlin(level.Seed.Copyx(12));
	}

	public override void Decorate(Level level, Chunk chunk, int x, int y)
	{
		if (y > chunk.GetSurface(x)) return;

		BlockState state = chunk.GetBlock(x, y);

		if (!state.Is(Groups.Carvable))
			return;

		float n1 = noise1.Generate(x / 16f, y / 12f, 1);
		float n2 = noise2.Generate(x / 16f, y / 16f, 1);
		float n3 = noise3.Generate(x / 24f, y / 16f, 1);
		float ck = noise4.Generate(x / 64f, 1, 1);

		if (y > Chunk.YOfSea - Chunk.ScaleOfCrust)
			n2 -= (y - Chunk.YOfSea + Chunk.ScaleOfCrust) / 128f / ck;

		if ((n1 < 0.25f || n1 > 0.85f || n3 > 0.52f && n3 < 0.6f) && n2 > 0.2f)
			chunk.SetBlock(BlockState.Empty, x, y);

		if (n2 > 0.66f)
		{
			chunk.SetBlock(BlockState.Empty, x, y);
			
			if (y < 16)
				chunk.SetLiquid(new LiquidStack(Liquids.Lava, Liquid.MaxAmount), x, y);
			else if (n1 < 0.35f)
				chunk.SetLiquid(new LiquidStack(Liquids.Water, Liquid.MaxAmount), x, y);
		}
	}

}
