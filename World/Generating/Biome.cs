using Ethla.Api;
using Ethla.World.Voxel;
using Spectrum.Core.Manage;

namespace Ethla.World.Generating;

public class Biome : IdHolder
{

	public static Biome Unknown => ModRegistry.Biomes.DefaultValue;

	public const int LayerFoliage = 0;
	public const int LayerDirt = 1;
	public const int LayerGravel = 2;
	public const int LayerStone = 3;

	public float Activeness;
	public float Continent;
	public float Depth;
	public float Rainfall;
	public float Rarity;
	public float Temperature;
	public BlockState[] BiomeStates = new BlockState[4];
	public Wall[] BiomeWalls = new Wall[4];

}
