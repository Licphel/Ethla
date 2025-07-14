using Ethla.World;
using Ethla.World.Generating;
using Ethla.World.Voxel;
using Spectrum.Maths.Random;

namespace Ethla.Common.Generating;

public class DecoratorPlants : Decorator
{

	private readonly Selector<BlockState> loot = new Selector<BlockState>().Put(Blocks.Flower.MakeState(), 5).Put(Blocks.Grass.MakeState(), 10).Put(Blocks.Bush.MakeState(), 3);

	public override DecoratorType Type => DecoratorType.Surface;
	public override int SurfaceOffset => 1;

	public override void Decorate(Level level, Chunk chunk, int x, int y)
	{
		if (Seed.NextFloat() < 0.75f && chunk.GetBlock(x, y).GetShape() == BlockShape.Vacuum && chunk.GetBlock(x, y - 1).Is(Groups.Dirt))
		{
			BlockState block = Seed.Select(loot.Objects);
			chunk.SetBlock(block, x, y);
		}
	}

}
