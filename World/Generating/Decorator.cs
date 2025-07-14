using Spectrum.Maths.Random;

namespace Ethla.World.Generating;

public abstract class Decorator
{

	protected Seed Seed;

	public virtual DecoratorType Type => DecoratorType.Waiting;
	public virtual int SurfaceOffset => 0;

	public virtual void InitSeed(Level level, Chunk chunk)
	{
		Seed = chunk.GetUniqSeed();
	}

	public abstract void Decorate(Level level, Chunk chunk, int x, int y);

}

public enum DecoratorType
{

	Following, //Run while filling basic blocks.
	Waiting, //Run after filling.
	Surface //Only run on surface after filling.

}
