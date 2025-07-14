namespace Ethla.World.Iteming;

public class Slot
{

	public ItemContainer Inv;
	public int InvIndex;
	public bool PlayerSide;
	public bool Storage;
	public int Uuid;

	public int X, Y;

	public Slot(int idx, int x, int y, bool playerside, bool store)
	{
		InvIndex = idx;
		X = x;
		Y = y;
		PlayerSide = playerside;
		Storage = store;
	}

	public ItemStack Stack
	{
		get => Inv.Get(InvIndex);
		set => Inv.Set(InvIndex, value);
	}

	public void Bind(ItemContainer inv, int uuid)
	{
		Inv = inv;
		Uuid = uuid;
	}

}
