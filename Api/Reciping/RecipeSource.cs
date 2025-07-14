using Ethla.World.Iteming;

namespace Ethla.Api.Reciping;

public interface RecipeSource
{

	ItemContainer ItemContainer { get; }

	bool IsResultDestinationAccessible(ItemStack stack, Recipe recipe);

	void CallAssemble(ItemStack stack, Recipe recipe);

}
