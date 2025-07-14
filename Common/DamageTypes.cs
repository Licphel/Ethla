using Ethla.Api;
using Ethla.World.Mob;
using Spectrum.Core.Manage;

namespace Ethla.Common;

public class DamageTypes
{

	public static readonly IdMap<DamageType> Registry = ModRegistry.DamageTypes;

	public static DamageType Unknown = Registry.RegisterDefaultValue("ethla:unknown", new DamageType());
	public static DamageType Mechanic = Registry.Register("ethla:mechanic", new DamageType());
	public static DamageType Electric = Registry.Register("ethla:electric", new DamageType());
	public static DamageType Erosive = Registry.Register("ethla:erosive", new DamageType());
	public static DamageType Flaming = Registry.Register("ethla:flaming", new DamageType());
	public static DamageType Biotic = Registry.Register("ethla:biotic", new DamageType());
	public static DamageType Elemental = Registry.Register("ethla:elemental", new DamageType());
	public static DamageType Divine = Registry.Register("ethla:divine", new DamageType());

}
