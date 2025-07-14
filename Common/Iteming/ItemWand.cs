using Ethla.World.Iteming;
using Ethla.World.Physics;
using Spectrum.Maths;

namespace Ethla.Common.Iteming;

public class ItemWand : ItemProjector
{

	public ItemWand(Item ammo, Vector2 powerRange, Func<Prefab> castObject, float cd) : base(ammo, powerRange, castObject, cd)
	{
	}

	public override ItemStack OnStackInteracting(ItemStack stack, ItemStack interactor)
	{
		if (interactor.Is(Groups.Fuel))
		{
			stack.EnsuredCompound.Set("type", stack.EnsuredCompound.GetOrDefault("type", 0) + 1);
			interactor.Grow(-1);
		}

		if (interactor.Is(Blocks.Dirt.Item))
		{
			stack.EnsuredCompound.Set("gem", stack.EnsuredCompound.GetOrDefault("gem", 0) + 1);
			interactor.Grow(-1);
		}

		if (interactor.Is(Blocks.Rock.Item))
		{
			stack.EnsuredCompound.Set("deco", stack.EnsuredCompound.GetOrDefault("deco", 0) + 1);
			interactor.Grow(-1);
		}

		return interactor;
	}

	public override int GetRarity(ItemStack stack)
	{
		return 5;
	}

}
