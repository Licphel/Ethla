using Ethla.Common.Mob;
using Ethla.World.Iteming;

namespace Ethla.Api.Reciping;

public class RecipeSourcePlayer : RecipeSource
{

	private readonly EntityPlayer player;

	public RecipeSourcePlayer(EntityPlayer player)
	{
		this.player = player;
	}

	public ItemContainer ItemContainer => player.Inventory;

	public void CallAssemble(ItemStack stack, Recipe recipe)
	{
		if (player.OpenContainer.Pickup.IsEmpty)
			player.OpenContainer.Pickup = stack;
		else
			player.OpenContainer.Pickup.Merge(stack);

		foreach (RecipeMatcher m in recipe.GetMatchers()) m.Consume(this);
	}

	public bool IsResultDestinationAccessible(ItemStack stack, Recipe recipe)
	{
		return player.OpenContainer.Pickup.CanMergeFully(stack);
	}

}
