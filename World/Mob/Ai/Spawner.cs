using Ethla.Api;
using Ethla.World.Physics;
using Spectrum.Core.Manage;
using Spectrum.Maths.Random;

namespace Ethla.World.Mob.Ai;

public static class Spawner
{

	public static Dictionary<Id, int> Counter = new Dictionary<Id, int>();

	public static void RefCount(Entity e)
	{
		Id id = e.Prefab.Uid;
		if (!Counter.ContainsKey(id))
			Counter[id] = 0;
		Counter[id]++;
	}

	public static void DecCount(Entity e)
	{
		Id id = e.Prefab.Uid;
		if (!Counter.TryGetValue(id, out int value))
			throw new Exception($"Entity counter failed when counting '{id}'.");
		Counter[id] = --value;
	}

	public static void TryGenerate(Chunk chunk)
	{
		List<SpawnEntry> spawnEntries = ModRegistry.SpawnEntries.IdList;

		SpawnEntry entry = Seed.Global.Select(spawnEntries);

		if (Counter.GetValueOrDefault(entry.Prefab.Uid, 0) >= entry.Cap)
			return;

		Pos pos = new PrecisePos(chunk.Coord * 16 + Seed.Global.NextFloat(0, 16), Seed.Global.NextFloat(0, Chunk.Height));

		if (entry.IsValid(chunk.Level, pos))
		{
			Entity entity = entry.Prefab.Instantiate();
			chunk.Level.SpawnEntity(entity, pos);
		}
	}

}
