using Ethla.Api;
using Ethla.Common.Generating;
using Ethla.World.Generating;
using Spectrum.Core.Manage;

namespace Ethla.Common;

public class Biomes
{

	public static readonly IdMap<Biome> Registry = ModRegistry.Biomes;

	public static Biome Unknown = Registry.RegisterDefaultValue("ethla:unknown", new Biome());
	public static Biome Forest = Registry.Register("ethla:forest", new BiomeForest());
	public static Biome Desert = Registry.Register("ethla:desert", new BiomeDesert());
	public static Biome Cave = Registry.Register("ethla:cave", new BiomeCave());

}
