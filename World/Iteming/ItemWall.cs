using System;
using Ethla.World.Physics;
using Ethla.World.Voxel;

namespace Ethla.World.Iteming;

public class ItemWall : Item
{

    private readonly Wall placement;
	
	public ItemWall(Wall wall)
	{
		placement = wall;
	}

	public override float CastLight(ItemStack stack, byte pipe)
	{
		return GetWallPlaced(stack).CastLight(pipe, 0, 0);
	}

	public override string GetLocalizationName(ItemStack stack)
	{
		return GetWallPlaced(stack).GetLocalizationName();
	}

	public override InterResult OnClickBlock(ItemStack stack, Entity entity, Level level, Pos pos, bool sim = false)
	{
		if (!level.IsAccessible(pos))
			return InterResult.Pass;

        Wall place = GetWallPlaced(stack);
        Wall prev = level.GetWall(pos);

		if (!stack.IsEmpty)
		{
			if (prev.CanBeReplace() && !prev.Equals(place))
			{
				if (!sim)
				{
					level.SetWall(place, pos);
					stack.Count--;
				}

				return InterResult.Blocked;
			}

			return InterResult.Pass;
		}

		return InterResult.Pass;
	}

	public virtual Wall GetWallPlaced(ItemStack stack)
	{
		return placement;
	}

	public override int GetRarity(ItemStack stack)
	{
		return GetWallPlaced(stack).Property.Rarity;
	}

}
