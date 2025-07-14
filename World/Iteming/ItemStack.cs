using Ethla.Api;
using Ethla.Api.Grouping;
using Ethla.Util;
using Ethla.World.Physics;
using Spectrum.Graphic.Text;
using Spectrum.IO.Bin;

namespace Ethla.World.Iteming;

public sealed class ItemStack
{

	// Do not use instantiate. It will throw an exception.
	public static readonly ItemStack Empty = new ItemStack(Item.Empty, 0);
	public BinaryCompound Compound;
	public int Count;
	private Item item;

	internal ItemStack(Item item, int count = 1)
	{
		this.item = item;
		Count = count;
	}

	public Item Item => Count <= 0 ? Item.Empty : item;
	public BinaryCompound EnsuredCompound => Compound ?? (Compound = BinaryCompound.New());
	public bool IsEmpty => Count <= 0 || Item == Item.Empty || Item == null || this == Empty;

	//Instance Relative

	public int MaxStackSize => Item.GetStackSize(this);

	public override bool Equals(object obj)
	{
		return obj is ItemStack state && IsCompatible(state);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Item);
	}

	//Cpy

	public ItemStack Copy(int count = -1)
	{
		if (IsEmpty)
			return Empty;

		ItemStack stack = new ItemStack(item);
		stack.ValueCopy(this, count);
		return stack;
	}

	public void ValueCopy(ItemStack stack, int count = -1)
	{
		if (IsEmpty)
			throw new Exception("Cannot operate on an empty stack.");

		item = stack.item;
		//It is worth mentioning that, we should not take the field 'Item'.
		//When the last one stack is picked, it needs to be kept.
		if (stack.Compound != null)
			Compound = stack.Compound.Copy();
		else
			Compound = null;

		Count = count == -1 ? stack.Count : count;
	}

	//Operations

	public ItemStack Merge(ItemStack s, bool simulate = false)
	{
		if (IsEmpty)
			throw new Exception("Cannot operate on an empty stack.");

		int ss = MaxStackSize;

		s = s.Copy();

		if (!IsCompatible(s)) return s;

		int c = s.Count;
		if (c + Count <= ss)
		{
			if (!simulate) Count += c;

			s.Count = 0;
		}
		else if (c + Count > ss)
		{
			int add = ss - Count;
			if (!simulate) Count = ss;

			s.Count -= add;
		}

		return s;
	}

	public ItemStack Take(int c)
	{
		if (IsEmpty)
			throw new Exception("Cannot operate on an empty stack.");

		if (Count < c)
		{
			Count = 0;
			return Copy(Count);
		}

		ItemStack newstk = Copy(c);
		Count -= c;
		return newstk;
	}

	public void Grow(int c)
	{
		if (IsEmpty)
			throw new Exception("Cannot operate on an empty stack.");

		Count += c;
	}

	public ItemStack SpiltHalf()
	{
		if (IsEmpty)
			throw new Exception("Cannot operate on an empty stack.");

		int count0 = Count % 2 == 0 ? Count / 2 : (Count + 1) / 2;
		return Take(count0);
	}

	//Checks

	public bool IsDataEqual(ItemStack stack)
	{
		if (stack.Compound == null && Compound == null) return true;
		if (stack.Compound != null && Compound != null) return stack.Compound.Compare(Compound);

		return false;
	}

	public bool IsCompatible(ItemStack stack)
	{
		return IsEmpty || stack.Item == Item && IsDataEqual(stack);
	}

	public bool CanMergeFully(ItemStack stack)
	{
		return Count + stack.Count <= MaxStackSize && IsCompatible(stack);
	}

	public bool CanMergePartly(ItemStack stack)
	{
		return Count <= MaxStackSize && IsCompatible(stack);
	}

	public bool Is(ItemStack stack)
	{
		return Count == stack.Count && IsCompatible(stack) && Item == stack.Item;
	}

	public bool Is(Group group)
	{
		return group.Contains(Item);
	}

	// Instance Relative

	public float CastLight(byte pipe)
	{
		return Item.CastLight(this, pipe);
	}

	public string GetLocalizationName()
	{
		return Item.GetLocalizationName(this);
	}

	public Tier GetTier(ItemStack stack)
	{
		return Item.GetTier(this);
	}

	public ItemToolType GetToolType()
	{
		return Item.GetToolType(this);
	}

	public float GetToolEfficiency()
	{
		return Item.GetToolEfficiency(this);
	}

	public void GetTooltips(List<Lore> tooltips)
	{
		if (IsEmpty)
			return;
		Item.GetTooltips(this, tooltips);
	}

	public InterResult OnClickBlock(Entity entity, Level level, Pos pos, bool sim = false)
	{
		return Item.OnClickBlock(this, entity, level, pos, sim);
	}

	public bool IsRemoteUsage()
	{
		return Item.IsRemoteUsage(this);
	}

	public InterResult OnUseItem(Entity entity, Level level, Pos pos, bool sim = false)
	{
		return Item.OnUseItem(this, entity, level, pos, sim);
	}

	public ItemStack OnStackInteracting(ItemStack interactor)
	{
		return Item.OnStackInteracting(this, interactor);
	}

	//Codec

	public static ItemStack Deserialize(BinaryCompound compound)
	{
		string type = compound.Get<string>("type");
		int count = compound.Get<int>("count");

		Item item = ModRegistry.Items[type];

		if (item == null || item == Item.Empty) return Empty;

		ItemStack stack = item.MakeStack(count);
		if (compound.Has("data")) stack.Compound = compound.GetCompoundSafely("compound");

		return stack;
	}

	public static BinaryCompound Serialize(ItemStack stack)
	{
		BinaryCompound compound = BinaryCompound.New();
		Serialize(stack, compound);
		return compound;
	}

	public static void Serialize(ItemStack stack, BinaryCompound compound)
	{
		compound.Set("type", stack.Item.Uid.ToString());
		compound.Set("count", stack.Count);

		if (stack.Compound != null)
			compound.Set("compound", stack.Compound);
	}

}
