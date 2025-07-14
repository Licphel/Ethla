using Ethla.Util;

namespace Ethla.World.Iteming;

public class ItemProperty
{

	public float[] Light = [0, 0, 0];
	public int MaxStackSize = 30;
	public int Rarity;
	public bool RemoteUsage;
	public Tier Tier = Tier.None;
	public ItemToolType ToolType = ItemToolType.None;

}
