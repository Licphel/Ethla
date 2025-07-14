using Ethla.World.Physics;
using Ethla.World.Voxel;
using Spectrum.Core.Manage;
using Spectrum.Maths.Shapes;

namespace Ethla.World.Iteming;

public class ItemBlock : Item
{

	private readonly Block placement;
	
	public ItemBlock(Block block)
	{
		placement = block;
	}

	public override float CastLight(ItemStack stack, byte pipe)
	{
		return GetBlockPlaced(stack).MakeState().CastLight(pipe, 0, 0);
	}

	public override string GetLocalizationName(ItemStack stack)
	{
		return GetBlockPlaced(stack).GetLocalizationName(BlockState.Empty);
	}

	public override InterResult OnClickBlock(ItemStack stack, Entity entity, Level level, Pos pos, bool sim = false)
	{
		if (!level.IsAccessible(pos))
			return InterResult.Pass;

		BlockState state = GetStateForPlacing(stack, entity, level, pos);
		BlockState prev = level.GetBlock(pos);

		if (!stack.IsEmpty)
		{
			if (prev.CanBeReplace() && !prev.Equals(state) && !CheckEntitiesAnyInteractWithBlock(level, state, pos) && state.CheckSustainability(level, new BlockPos(pos)))
			{
				if (!sim)
				{
					level.SetBlock(state, pos);
					stack.Count--;
				}

				return InterResult.Blocked;
			}

			return InterResult.Pass;
		}

		return InterResult.Pass;
	}

	public virtual BlockState GetStateForPlacing(ItemStack stack, Entity entity, Level level, Pos pos)
	{
		Block block = GetBlockPlaced(stack);
		return block.GetStateForPlacing(level, entity, pos, stack);
	}

	public virtual Block GetBlockPlaced(ItemStack stack)
	{
		return placement;
	}

	public override int GetRarity(ItemStack stack)
	{
		return GetBlockPlaced(stack).Property.Rarity;
	}

	public static bool CheckEntitiesAnyInteractWithBlock(Level level, BlockState state, Pos pos)
	{
		List<Entity> lst = new List<Entity>();
		Quad aabb = new Quad();
		aabb.LocateCentral(pos.X, pos.Y);
		aabb.Resize(16, 16);
		level.GetNearbyEntities(lst, aabb);

		foreach (Entity e in lst)
			if (state.GetSilhouette(e).Interacts(e.Bound, pos.BlockX, pos.BlockY))
				return true;

		return false;
	}

}
