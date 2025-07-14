using Spectrum.Core.Manage;
using Spectrum.Maths;
using Spectrum.Maths.Random;

namespace Ethla.World.Generating;

public abstract class Feature : IdHolder
{

	public bool IsSurfacePlaced = false;
	public int MaxCoverChunks = 1;
	public Vector2 Range = new Vector2(0, Chunk.YOfSea);
	public bool RangedGuassian;
	public float TryChance = 1;
	public float TryTimesPerChunk = 0;

	public abstract bool IsPlacable(Level level, int x, int y, Seed seed);

	public abstract void Place(Level level, int x, int y, Seed seed);

}
