using Ethla.Common;
using Ethla.World.Iteming;
using Spectrum.Core.Manage;
using Spectrum.IO.Bin;

namespace Ethla.Api.Reciping;

public class RecipeSmelt : RecipeSimple
{

	public float Cooktime;

	public RecipeSmelt()
	{
		// Only drain input slots.
		SlotMatchFilter = i => i >= 0 && i <= 2;
	}

	public ItemStack Output0 => Outputs[0];

	public override RecipeCategory Category => Recipes.Furnace;

	public override bool Assemble(RecipeSource source)
	{
		if (!Matches(source)) return false;
		if (source.IsResultDestinationAccessible(Output0, this))
		{
			source.CallAssemble(Output0.Copy(), this);
			return true;
		}

		return false;
	}

	public override void Read(BinaryCompound compound, Id id)
	{
		base.Read(compound, id);
		Cooktime = compound.GetOrDefault("cooktime", 1);
	}

}
