using Spectrum.Maths.Random;

namespace Ethla.World.Generating;

public abstract class Generator
{

	public readonly List<Decorator> Decorators = new List<Decorator>();
	public readonly List<Decorator> DecoratorsFollowing = new List<Decorator>();
	public readonly List<Decorator> DecoratorsSurface = new List<Decorator>();
	public readonly List<Decorator> DecoratorsWaiting = new List<Decorator>();
	public readonly List<PostProcessor> Processors = new List<PostProcessor>();

	public void AddDecorator(Decorator dec)
	{
		switch (dec.Type)
		{
			case DecoratorType.Surface:
				DecoratorsSurface.Add(dec);
				break;
			case DecoratorType.Waiting:
				DecoratorsWaiting.Add(dec);
				break;
			case DecoratorType.Following:
				DecoratorsFollowing.Add(dec);
				break;
		}

		Decorators.Add(dec);
	}

	public void AddProcessor(PostProcessor proc)
	{
		Processors.Add(proc);
	}

	public abstract void SetSeed(Seed seed);

	public abstract void Provide(int coord);

	public abstract bool ProvideAsync(int coord);

	public abstract Chunk ProvideEmpty(int coord);

	public abstract void GetLocationData(int x, int y, int surface, GenerateContext ctx);

}
