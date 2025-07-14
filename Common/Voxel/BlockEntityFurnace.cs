using Ethla.Api.Reciping;
using Ethla.World;
using Ethla.World.Iteming;
using Ethla.World.Voxel;
using Spectrum.Core;
using Spectrum.IO.Bin;
using Spectrum.Maths;

namespace Ethla.Common.Voxel;

public class BlockEntityFurnace : BlockEntity, Tickable
{

	public float Cooktime, MaxTime = 1;
	public float Fuel, MaxFuel = 1;

	public Inventory Inv = new Inventory(5);
	public RecipeSmelt Recipe;

	public BlockEntityFurnace(BlockState state, Level level, BlockPos pos) : base(state, level, pos)
	{
	}

	public void Tick()
	{
		RecipeSource src = new RecipeSourceInventory(Inv, new Vector2(4, 4));

		Recipe = null;
		IEnumerable<Recipe> recipes = RecipeManager.GetRecipes(Recipes.Furnace);

		foreach (Recipe r in recipes)
			if (r.Matches(src))
			{
				Recipe = (RecipeSmelt)r;
				break;
			}

		if (Recipe != null)
		{
			MaxTime = Recipe.Cooktime;

			bool ac = src.IsResultDestinationAccessible(Recipe.Output0, Recipe);

			if (Fuel <= 0)
			{
				ItemStack stack = Inv[3];
				if (Recipe != null && stack.Is(Groups.Fuel) && ac)
				{
					Fuel = MaxFuel = Groups.Fuel.Valueof(stack.Item);
					Inv[3].Grow(-1);
				}
			}

			if (Fuel > 0 && ac)
				Cooktime += Time.Delta;
			else
				Cooktime -= Time.Delta;

			Cooktime = Math.Clamp(Cooktime, 0, float.MaxValue);

			if (Cooktime - Mathf.Tolerance >= Recipe.Cooktime)
			{
				Cooktime = 0;
				Recipe.Assemble(src);
				Recipe = null;
			}
		}
		else
		{
			Cooktime = Math.Clamp(Cooktime - 1, 0, MaxTime);
		}

		Fuel = Math.Clamp(Fuel - Time.Delta, 0, MaxFuel);
	}

	public override void OverrideBlockDrops(List<ItemStack> stacks)
	{
		base.OverrideBlockDrops(stacks);

		foreach (ItemStack stk in Inv)
			if (!stk.IsEmpty)
				stacks.Add(stk);
	}

	public override void Read(BinaryCompound compound)
	{
		base.Read(compound);
		Inv = Inventory.Deserialize(compound.GetCompoundSafely("cinv"));
		Cooktime = compound.Get<float>("cooktime");
		Fuel = compound.Get<float>("fuel");
		MaxFuel = compound.Get<float>("maxfuel");
	}

	public override void Write(BinaryCompound compound)
	{
		base.Write(compound);
		Inventory.Serialize(Inv, compound.NewScope("cinv"));
		compound.Set("cooktime", Cooktime);
		compound.Set("fuel", Fuel);
		compound.Set("maxfuel", MaxFuel);
	}

}
