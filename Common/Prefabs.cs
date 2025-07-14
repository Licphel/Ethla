using Ethla.Api;
using Ethla.Common.Mob;
using Ethla.Common.Particles;
using Ethla.World.Physics;
using Spectrum.Core.Manage;

namespace Ethla.Common;

public class Prefabs
{

	public static readonly IdMap<Prefab> Registry = ModRegistry.Prefabs;

	public static Prefab Null = Registry.RegisterDefaultValue("ethla:null", null);
	public static Prefab Player = Registry.Register("ethla:player", new Prefab(typeof(EntityPlayer)));
	public static Prefab Item = Registry.Register("ethla:item", new Prefab(typeof(EntityItem)));
	public static Prefab MagicOrb = Registry.Register("ethla:magic_orb", new Prefab(typeof(EntityOrb)));
	public static Prefab Arrow = Registry.Register("ethla:arrow", new Prefab(typeof(EntityArrow)));
	public static Prefab Worm = Registry.Register("ethla:worm", new Prefab(typeof(EntityWorm)));
	public static Prefab ParticleBlockDust = Registry.Register("ethla:particle_block_dust", new Prefab(typeof(ParticleBlockDust)));

}
