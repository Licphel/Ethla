using Ethla.Api;
using Ethla.World.Mob.Ai;
using Spectrum.Core.Manage;

namespace Ethla.Common;

public class SpawnEntries
{

	public static readonly IdMap<SpawnEntry> Registry = ModRegistry.SpawnEntries;

	public static SpawnEntry Worm = Registry.Register("ethla:worm", new SpawnEntry(Prefabs.Worm, 8, SpawnEntry.Anywhere, SpawnEntry.StandFirm));

}
