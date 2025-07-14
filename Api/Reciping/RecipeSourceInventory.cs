using Ethla.World.Iteming;
using Spectrum.Maths;

namespace Ethla.Api.Reciping;

public class RecipeSourceInventory : RecipeSource
{

	private readonly Inventory inv;
	private readonly Vector2 range;

	public RecipeSourceInventory(Inventory inv, Vector2 optRange)
	{
		this.inv = inv;
		range = optRange;
	}

	public ItemContainer ItemContainer => inv;

	public void CallAssemble(ItemStack stack, Recipe recipe)
	{
		inv.Add(stack, range.Xi, range.Yi);

		foreach (RecipeMatcher m in recipe.GetMatchers()) m.Consume(this);
	}

	public bool IsResultDestinationAccessible(ItemStack stack, Recipe recipe)
	{
		return inv.Add(stack, range.Xi, range.Yi, true).IsEmpty;
	}

}
