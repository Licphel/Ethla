using Ethla.Api;
using Ethla.Common.Voxel;
using Ethla.World.Voxel;
using Spectrum.Core.Manage;

namespace Ethla.Common;

public class Liquids
{

	public static readonly IdMap<Liquid> Registry = ModRegistry.Liquids;

	public static Liquid Empty = Registry.RegisterDefaultValue("ethla:empty", new Liquid());
	public static Liquid Water = Registry.Register("ethla:water", new LiquidWater());
	public static Liquid Lava = Registry.Register("ethla:lava", new LiquidLava());

}
