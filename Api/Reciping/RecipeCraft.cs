using Ethla.Common;
using Ethla.World.Iteming;

namespace Ethla.Api.Reciping;

public class RecipeCraft : RecipeSimple
{

	public ItemStack Output0 => Outputs[0];

	public override RecipeCategory Category => Recipes.Handcraft;

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

}
