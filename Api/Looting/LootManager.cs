using Spectrum.Core.Manage;

namespace Ethla.Api.Looting;

public class LootManager
{

	public static readonly Dictionary<Id, Loot> WallDict = new Dictionary<Id, Loot>();
	public static readonly Dictionary<Id, Loot> BlockDict = new Dictionary<Id, Loot>();
	public static readonly Dictionary<Id, Loot> EntityDict = new Dictionary<Id, Loot>();

	public static Loot GetForWall(Id key)
	{
		return WallDict.GetValueOrDefault(key, null);
	}

	public static Loot GetForBlock(Id key)
	{
		return BlockDict.GetValueOrDefault(key, null);
	}

	public static Loot GetForEntity(Id key)
	{
		return EntityDict.GetValueOrDefault(key, null);
	}

}
