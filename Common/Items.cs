using Ethla.Api;
using Ethla.Common.Iteming;
using Ethla.World.Iteming;
using Spectrum.Core.Manage;
using Spectrum.Maths;

namespace Ethla.Common;

public class Items
{

	public static readonly IdMap<Item> Registry = ModRegistry.Items;

	public static Item Empty = Registry.RegisterDefaultValue("ethla:empty", new Item());
	public static Item Coal = Registry.Register("ethla:coal", new Item());
	public static Item CopperIngot = Registry.Register("ethla:copper_ingot", new Item());
	public static Item IronIngot = Registry.Register("ethla:iron_ingot", new Item());
	public static Item GoldIngot = Registry.Register("ethla:gold_ingot", new Item());
	public static Item ManatiumIngot = Registry.Register("ethla:manatium_ingot", new Item());
	public static Item IronPickaxe = Registry.Register("ethla:iron_pickaxe", new Item());
	public static Item IronAxe = Registry.Register("ethla:iron_axe", new Item());
	public static Item IronShovel = Registry.Register("ethla:iron_shovel", new Item());
	public static Item IronHammer = Registry.Register("ethla:iron_hammer", new Item());
	public static Item GoldenPickaxe = Registry.Register("ethla:golden_pickaxe", new Item());
	public static Item GoldenAxe = Registry.Register("ethla:golden_axe", new Item());
	public static Item GoldenShovel = Registry.Register("ethla:golden_shovel", new Item());
	public static Item ManatiumPickaxe = Registry.Register("ethla:manatium_pickaxe", new Item());
	public static Item ManatiumAxe = Registry.Register("ethla:manatium_axe", new Item());
	public static Item ManatiumShovel = Registry.Register("ethla:manatium_shovel", new Item());
	public static Item Arrow = Registry.Register("ethla:arrow", new Item());
	public static Item Bow = Registry.Register("ethla:bow", new ItemProjector(Arrow, new Vector2(25, 30), () => Prefabs.Arrow, 1));
	public static Item Wand = Registry.Register("ethla:wand", new ItemWand(Item.Empty, new Vector2(20, 20), () => Prefabs.MagicOrb, 0.05f));

}
