using Ethla.Api;
using Ethla.Common.Generating;
using Ethla.World.Generating;
using Spectrum.Core.Manage;

namespace Ethla.Common;

public class Features
{

	public static readonly IdMap<Feature> Registry = ModRegistry.Features;

	public static Feature Null = Registry.RegisterDefaultValue("ethla:null", null);
	public static Feature BirchTree = Registry.Register("ethla:birch_tree", new FeatureTreeBirch());
	public static Feature MapleTree = Registry.Register("ethla:maple_tree", new FeatureTreeMaple());
	public static Feature Vine = Registry.Register("ethla:vine", new FeatureVine());
	public static Feature CoalOreCluster = Registry.Register("ethla:coal_ore_cluster", new FeatureOre(3, 5, () => Blocks.CoalOre.MakeState()));
	public static Feature IronOreCluster = Registry.Register("ethla:iron_ore_cluster", new FeatureOre(2, 5, () => Blocks.IronOre.MakeState()));
	public static Feature GoldOreCluster = Registry.Register("ethla:gold_ore_cluster", new FeatureOre(1, 3, () => Blocks.GoldOre.MakeState()));
	public static Feature GravelCluster = Registry.Register("ethla:gravel_cluster", new FeatureOre(5, 3, () => Blocks.Gravel.MakeState()));
	public static Feature ClayCluster = Registry.Register("ethla:clay_cluster", new FeatureClay(5, 5, () => Blocks.Clay));
	public static Feature TurfCluster = Registry.Register("ethla:turf_cluster", new FeatureClay(5, 5, () => Blocks.Turf));
	public static Feature Cactus = Registry.Register("ethla:cactus", new FeatureCactus());
	public static Feature Canyon = Registry.Register("ethla:canyon", new FeatureCanyon());

}
