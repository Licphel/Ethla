using Ethla.World.Generating;
using Spectrum.Core.Manage;

namespace Ethla.World;

public class LevelType
{

	public Id Id;
	public Func<Level, Generator> ProviderGenerator;

	public LevelType(Id id, Func<Level, Generator> provgenerator)
	{
		Id = id;
		ProviderGenerator = provgenerator;
	}

}
