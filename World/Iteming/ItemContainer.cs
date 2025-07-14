using System.Collections;
using Spectrum.Utils;

namespace Ethla.World.Iteming;

public abstract class ItemContainer : IEnumerable<ItemStack>
{

	public ItemStack this[int index]
	{
		get => Get(index);
		set => Set(index, value);
	}

	public abstract int Count { get; }

	public abstract ItemStack[] Array { get; }

	public IEnumerator<ItemStack> GetEnumerator()
	{
		return new List<ItemStack>(Array).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public abstract void Set(int i, ItemStack s);

	//IMPL DEF
	public ItemStack Add(ItemStack s, bool simulate = false)
	{
		return Add(s, 0, Count - 1, simulate);
	}

	public abstract ItemStack Add(ItemStack s, int startSlot, int endSlot, bool simulate = false);

	//IMPL DEF
	public void Clear()
	{
		for (int i = 0; i < Count; i++) Set(i, ItemStack.Empty);
	}

	//IMPL DEF
	public List<ItemStack> Extract(ItemStack s, bool simulate = false)
	{
		return Extract(s, 0, Count - 1, simulate);
	}

	//IMPL DEF
	public List<ItemStack> Extract(ItemStack s, int startSlot, int endSlot, bool simulate = false)
	{
		return Extract(s.Count, i => i.Item == s.Item, startSlot, endSlot, simulate);
	}

	//IMPL DEF
	public List<ItemStack> Extract(int count, Predicate<ItemStack> filter, bool simulate = false)
	{
		return Extract(count, filter, 0, Count - 1, simulate);
	}

	public abstract List<ItemStack> Extract(int count, Predicate<ItemStack> filter, int startSlot, int endSlot, bool simulate = false);

	public abstract bool IsAccessible(ItemStack s, int slot);

	public abstract ItemStack Get(int i);

	public abstract bool Contains(ItemStack item);

	//IMPL DEF

	//Completely raw copying.
	//If the #Set method has some additional effect, override this as well.
	public void Copy(ItemContainer container)
	{
		ItemStack[] s0 = Array;
		ItemStack[] s1 = container.Array;

		if (s0.Length != s1.Length) Logger.Warn("Inventory sizes are not equal. Copying failed.");

		System.Array.Copy(s1, s0, s0.Length);
	}

}
