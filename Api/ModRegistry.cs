using Ethla.Api.Reciping;
using Ethla.World.Generating;
using Ethla.World.Iteming;
using Ethla.World.Mob;
using Ethla.World.Mob.Ai;
using Ethla.World.Physics;
using Ethla.World.Voxel;
using Spectrum.Core.Manage;

namespace Ethla.Api;

public class ModRegistry
{

	public static readonly IdMap<Biome> Biomes = new IdMap<Biome>();
	public static readonly IdMap<Block> Blocks = new IdMap<Block>();
	public static readonly IdMap<BlockMaterial> BlockMaterials = new IdMap<BlockMaterial>();
	public static readonly IdMap<Wall> Walls = new IdMap<Wall>();
	public static readonly IdMap<Item> Items = new IdMap<Item>();
	public static readonly IdMap<Prefab> Prefabs = new IdMap<Prefab>();
	public static readonly IdMap<Feature> Features = new IdMap<Feature>();
	public static readonly IdMap<Liquid> Liquids = new IdMap<Liquid>();
	public static readonly IdMap<RecipeCategory> RecipeCategories = new IdMap<RecipeCategory>();
	public static readonly IdMap<SpawnEntry> SpawnEntries = new IdMap<SpawnEntry>();
	public static readonly IdMap<DamageType> DamageTypes = new IdMap<DamageType>();

}
