using Ethla.Api;
using Ethla.Api.Reciping;
using Spectrum.Core.Manage;

namespace Ethla.Common;

public class Recipes
{

	public static readonly IdMap<RecipeCategory> Registry = ModRegistry.RecipeCategories;

	public static RecipeCategory Null = Registry.RegisterDefaultValue("ethla:null", null);
	public static RecipeCategory Handcraft = Registry.Register("ethla:craft", new RecipeCategory(() => new RecipeCraft()));
	public static RecipeCategory Furnace = Registry.Register("ethla:smelt", new RecipeCategory(() => new RecipeSmelt()));

}
