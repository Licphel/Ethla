using Ethla.World.Iteming;
using Spectrum.Core.Manage;
using Spectrum.IO.Bin;

namespace Ethla.Api.Reciping;

public abstract class RecipeSimple : Recipe
{

	public List<RecipeMatcher> Matchers = new List<RecipeMatcher>();
	public List<ItemStack> Outputs = new List<ItemStack>();
	public Func<int, bool> SlotMatchFilter = i => true;

	public Id Path { get; private set; }

	public abstract RecipeCategory Category { get; }

	public virtual bool Matches(RecipeSource source)
	{
		foreach (RecipeMatcher matcher in Matchers)
			if (!matcher.Test(source, SlotMatchFilter))
				return false;
		return true;
	}

	public virtual void Read(BinaryCompound compound, Id id)
	{
		Path = id;

		BinaryList matchersComp = compound.GetListSafely("inputs");

		foreach (BinaryList c in matchersComp)
			Matchers.Add(RecipeMatcher.Read(c));

		BinaryList opComp = compound.GetListSafely("outputs");

		foreach (BinaryList c in opComp)
			Outputs.Add(RecipeDecodeHelper.StaticItem(c));
	}

	public abstract bool Assemble(RecipeSource source);

	public virtual List<RecipeMatcher> GetMatchers()
	{
		return Matchers;
	}

	public List<ItemStack> GetOutputs()
	{
		return Outputs;
	}

}
